import { useEffect, useState } from "react";
import { useLocation, Link as RouterLink, useSearchParams } from "react-router-dom";
import { Box, Heading, Text, Stack, Spinner, Link, Alert, AlertIcon, Button, HStack, useDisclosure } from "@chakra-ui/react";
import RoomSummary from "../components/RoomSummary";

type Event = {
    id: string;
    name: string;
    date: string;
    availableSpots: number;
    description: string;
    guestIds: string[];
    createdAt: string;
    updatedAt: string;
    price?: number;
    imageUrl?: string;
};

type Guest = {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    roomId?: string;
};

const API_BASE = "http://localhost:7071/api";

export default function Explore() {
    const location = useLocation();
    const state = location.state as { name?: string } | null;
    const fallbackName = state?.name ?? "";

    const [searchParams] = useSearchParams();
    const guestId = searchParams.get("guestId") ?? "";

    const [guest, setGuest] = useState<Guest | null>(null);
    const [events, setEvents] = useState<Event[]>([]);
    const [eventsLoading, setEventsLoading] = useState(true);
    const [eventsError, setEventsError] = useState<string | null>(null);

    const { isOpen: isRoomSummaryOpen, onOpen: onRoomSummaryOpen, onClose: onRoomSummaryClose } = useDisclosure();

    // Fetch guest (if guestId present)
    useEffect(() => {
        if (!guestId) return;
        fetch(`${API_BASE}/guests/${guestId}`)
            .then((res) => {
                if (!res.ok) throw new Error(`HTTP ${res.status}`);
                return res.json();
            })
            .then(setGuest)
            .catch((err) => console.error("Failed to fetch guest:", err));
    }, [guestId]);

    // Fetch events from backend
    useEffect(() => {
        async function fetchEvents() {
            try {
                setEventsLoading(true);
                setEventsError(null);
                const res = await fetch(`${API_BASE}/events`);

                if (!res.ok) throw new Error(`HTTP ${res.status}`);
                const data: Event[] = await res.json();
                setEvents(data ?? []);
            } catch (err: any) {
                console.error("Failed to fetch events:", err);
                setEventsError(err?.message ?? "Failed to fetch events");
            } finally {
                setEventsLoading(false);
            }
        }
        fetchEvents();
    }, []);

    if (eventsLoading) {
        return (
            <Box p="2rem" textAlign="center">
                <Spinner size="lg" />
            </Box>
        );
    }

    return (
        <Box p="2rem">
            <HStack justify="space-between" align="start" mb={6}>
                <Heading>
                    {guest
                        ? `Hi, ${guest.firstName} ${guest.lastName}! 👋`
                        : fallbackName
                            ? `Hi, ${fallbackName}! 👋`
                            : "Hi there 👋"}
                </Heading>
                
                {guest?.roomId && (
                    <Button colorScheme="blue" variant="outline" onClick={onRoomSummaryOpen}>
                        Room Tab
                    </Button>
                )}
            </HStack>

            {eventsError && (
                <Alert status="error" mb={4}>
                    <AlertIcon />
                    {eventsError}
                </Alert>
            )}

            <Heading size="md" mb={4}>Available Activities</Heading>

            {events.length === 0 ? (
                <Text>No activities found.</Text>
            ) : (
                <Stack spacing={4}>
                    {events.map((event) => {
                        const search = guestId ? `?guestId=${guestId}` : "";
                        const remainingSpots = event.availableSpots - (event.guestIds?.length ?? 0);
                        return (
                            <Link
                                as={RouterLink}
                                to={`/events/${event.id}${search}`} // keep guestId in URL
                                key={event.id}
                                style={{ textDecoration: "none" }}
                            >
                                <Box
                                    p={4}
                                    borderWidth="1px"
                                    borderRadius="lg"
                                    boxShadow="md"
                                    bg="white"
                                    _hover={{ bg: "blue.50" }}
                                >
                                    <Heading size="sm" mb={2}>{event.name}</Heading>
                                    <Text>
                                        <strong>Date:</strong> {new Date(event.date).toLocaleDateString()}
                                    </Text>
                                    <Text>
                                        <strong>Available Spots:</strong> {remainingSpots}
                                    </Text>
                                </Box>
                            </Link>
                        );
                    })}
                </Stack>
            )}

            <RoomSummary 
                isOpen={isRoomSummaryOpen} 
                onClose={onRoomSummaryClose} 
                guest={guest} 
            />
        </Box>
    );
}
