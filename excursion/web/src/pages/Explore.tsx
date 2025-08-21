import { useEffect, useState } from "react";
import { useLocation, Link as RouterLink } from "react-router-dom";
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

export default function Explore() {
    const location = useLocation();
    const state = location.state as { name?: string } | null;
    const userName = state?.name ?? "";

    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function fetchEvents() {
            try {
                const res = await fetch("/src/data/events.json"); // fetch from your path
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
                {userName ? `Hi, ${userName}! 👋` : "Hi there 👋"}
            </Heading>

            <Heading size="md" mb={4}>Available Activities</Heading>
            <Stack spacing={4}>
                {events.map((event) => (
                    <Link
                        as={RouterLink}
                        to={`/events/${event.id}`}
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
