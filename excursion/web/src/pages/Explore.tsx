import { useEffect, useState } from "react";
import { useLocation, Link as RouterLink, useSearchParams } from "react-router-dom";
import { Box, Heading, Text, Stack, Spinner, Link, Alert, AlertIcon, Button, HStack, VStack, useDisclosure } from "@chakra-ui/react";
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
    const [roommates, setRoommates] = useState<Guest[]>([]);
    const [events, setEvents] = useState<Event[]>([]);
    const [eventsLoading, setEventsLoading] = useState(true);
    const [eventsError, setEventsError] = useState<string | null>(null);

    const { isOpen: isRoomSummaryOpen, onOpen: onRoomSummaryOpen, onClose: onRoomSummaryClose } = useDisclosure();

    // Fetch guest and roommates (if guestId present)
    useEffect(() => {
        if (!guestId) return;
        
        async function fetchGuestAndRoommates() {
            try {
                // First fetch the guest
                const guestRes = await fetch(`${API_BASE}/guests/${guestId}`);
                if (!guestRes.ok) throw new Error(`HTTP ${guestRes.status}`);
                const guestData: Guest = await guestRes.json();
                setGuest(guestData);

                // If guest has a room, fetch roommates
                if (guestData.roomId) {
                    const roomRes = await fetch(`${API_BASE}/rooms/${guestData.roomId}`);
                    if (roomRes.ok) {
                        const roomData = await roomRes.json();
                        
                        // Fetch all guests in the room
                        if (roomData.guestIds && roomData.guestIds.length > 0) {
                            const roommatePromises = roomData.guestIds
                                .filter((id: string) => id !== guestId) // Exclude current guest
                                .map(async (id: string) => {
                                    const res = await fetch(`${API_BASE}/guests/${id}`);
                                    return res.ok ? await res.json() : null;
                                });
                            
                            const roommateData = await Promise.all(roommatePromises);
                            setRoommates(roommateData.filter(Boolean));
                        }
                    }
                }
            } catch (err) {
                console.error("Failed to fetch guest or roommates:", err);
            }
        }

        fetchGuestAndRoommates();
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
                        const totalSignedUp = event.guestIds?.length ?? 0;
                        const remainingSpots = event.availableSpots - totalSignedUp;
                        
                        // Check if current guest is signed up
                        const isCurrentGuestSignedUp = guest && event.guestIds?.includes(guest.id);
                        
                        // Check which roommates are signed up
                        const signedUpRoommates = roommates.filter(roommate => 
                            event.guestIds?.includes(roommate.id)
                        );
                        
                        return (
                            <Box
                                key={event.id}
                                p={4}
                                borderWidth="1px"
                                borderRadius="lg"
                                boxShadow="md"
                                bg="white"
                                position="relative"
                            >
                                {/* Status indicator */}
                                {isCurrentGuestSignedUp && (
                                    <Box
                                        position="absolute"
                                        top={2}
                                        right={2}
                                        bg="green.500"
                                        color="white"
                                        px={2}
                                        py={1}
                                        borderRadius="md"
                                        fontSize="xs"
                                        fontWeight="bold"
                                    >
                                        ✓ Signed up
                                    </Box>
                                )}
                                
                                <VStack align="stretch" spacing={3}>
                                    <Box>
                                        <Heading size="sm" mb={2}>{event.name}</Heading>
                                        <Text fontSize="sm" color="gray.600" mb={2}>
                                            {event.description}
                                        </Text>
                                    </Box>
                                    
                                    <HStack justify="space-between" fontSize="sm">
                                        <VStack align="start" spacing={1}>
                                            <Text>
                                                <strong>Date:</strong> {new Date(event.date).toLocaleDateString()}
                                            </Text>
                                            <Text>
                                                <strong>Time:</strong> {new Date(event.date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                            </Text>
                                        </VStack>
                                        <VStack align="end" spacing={1}>
                                            <Text color={remainingSpots > 0 ? "green.600" : "red.600"} fontWeight="medium">
                                                {remainingSpots > 0 
                                                    ? `${remainingSpots} spots left` 
                                                    : "Fully booked"}
                                            </Text>
                                            <Text fontSize="xs" color="gray.500">
                                                {totalSignedUp} of {event.availableSpots} taken
                                            </Text>
                                            {event.price && (
                                                <Text fontWeight="medium">
                                                    ${event.price.toFixed(2)}
                                                </Text>
                                            )}
                                        </VStack>
                                    </HStack>
                                    
                                    {/* Roommate status */}
                                    {signedUpRoommates.length > 0 && (
                                        <Text fontSize="sm" color="blue.600">
                                            👥 {signedUpRoommates.map(r => r.firstName).join(', ')} {signedUpRoommates.length === 1 ? 'is' : 'are'} signed up
                                        </Text>
                                    )}
                                    
                                    <HStack justify="space-between" align="center">
                                        <Button
                                            as={RouterLink}
                                            to={`/events/${event.id}${search}`}
                                            size="sm"
                                            colorScheme="blue"
                                            variant="outline"
                                        >
                                            View Details
                                        </Button>
                                        
                                        {isCurrentGuestSignedUp ? (
                                            <Text fontSize="sm" color="green.600" fontWeight="medium">
                                                You're signed up!
                                            </Text>
                                        ) : remainingSpots > 0 ? (
                                            <Button
                                                as={RouterLink}
                                                to={`/events/${event.id}${search}`}
                                                size="sm"
                                                colorScheme="green"
                                            >
                                                Sign Up
                                            </Button>
                                        ) : (
                                            <Text fontSize="sm" color="red.600" fontWeight="medium">
                                                Fully booked
                                            </Text>
                                        )}
                                    </HStack>
                                </VStack>
                            </Box>
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
