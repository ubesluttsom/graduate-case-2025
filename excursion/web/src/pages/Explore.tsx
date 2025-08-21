import { useEffect, useState } from "react";
import { useLocation, Link as RouterLink, useSearchParams } from "react-router-dom";
import { Box, Heading, Text, Stack, Spinner, Link } from "@chakra-ui/react";

type Event = {
    id: string;
    name: string;
    date: string;
    availableSpots: number;
    description: string;
    guestIds: string[];
    createdAt: string;
    updatedAt: string;
};

type Guest = {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
};

export default function Explore() {
    const location = useLocation();
    const state = location.state as { name?: string } | null;
    const fallbackName = state?.name ?? "";

    const [searchParams] = useSearchParams();
    const guestId = searchParams.get("guestId");

    const [guest, setGuest] = useState<Guest | null>(null);
    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState(true);

    // Fetch guest if guestId exists
    useEffect(() => {
        if (guestId) {
            fetch(`http://localhost:7071/api/guests/${guestId}`)
                .then((res) => res.json())
                .then(setGuest)
                .catch((err) => console.error("Failed to fetch guest:", err));
        }
    }, [guestId]);

    // Fetch events
    useEffect(() => {
        async function fetchEvents() {
            try {
                const res = await fetch("/src/data/events.json"); // static JSON fallback
                const data: Event[] = await res.json();
                setEvents(data);
            } catch (err) {
                console.error("Failed to fetch events:", err);
            } finally {
                setLoading(false);
            }
        }
        fetchEvents();
    }, []);

    if (loading) {
        return (
            <Box p="2rem" textAlign="center">
                <Spinner size="lg" />
            </Box>
        );
    }

    return (
        <Box p="2rem">
            <Heading mb={6}>
                {guest
                    ? `Hi, ${guest.firstName} ${guest.lastName}! 👋`
                    : fallbackName
                        ? `Hi, ${fallbackName}! 👋`
                        : "Hi there 👋"}
            </Heading>

            <Heading size="md" mb={4}>Available Activities</Heading>
            <Stack spacing={4}>
                {events.map((event) => (
                    <Link
                        as={RouterLink}
                        to={`/events/${event.id}?guestId=${guestId ?? ""}`} // 👈 pass guestId forward
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
                            <Text><strong>Date:</strong> {new Date(event.date).toLocaleDateString()}</Text>
                            <Text><strong>Available Spots:</strong> {event.availableSpots}</Text>
                        </Box>
                    </Link>
                ))}
            </Stack>
        </Box>
    );
}
