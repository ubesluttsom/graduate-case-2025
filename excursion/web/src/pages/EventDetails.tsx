import { useEffect, useState } from "react";
import { useParams, Link as RouterLink } from "react-router-dom";
import {
    Box,
    Heading,
    Text,
    Spinner,
    Button,
    HStack,
    Link
} from "@chakra-ui/react";

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

export default function EventDetails() {
    const { id } = useParams<{ id: string }>();
    const [event, setEvent] = useState<Event | null>(null);
    const [loading, setLoading] = useState(true);
    const [adults, setAdults] = useState(0);

    useEffect(() => {
        async function fetchEvents() {
            try {
                const res = await fetch("/src/data/events.json");
                const data: Event[] = await res.json();
                const found = data.find((e) => e.id === id);
                setEvent(found || null);
            } catch (err) {
                console.error("Failed to fetch event:", err);
            } finally {
                setLoading(false);
            }
        }
        fetchEvents();
    }, [id]);

    if (loading) {
        return (
            <Box p="2rem" textAlign="center">
                <Spinner size="lg" />
            </Box>
        );
    }

    if (!event) {
        return (
            <Box p="2rem">
                <Heading size="md">Event not found</Heading>
                <Link as={RouterLink} to="/explore" color="blue.500">
                    Back to Explore
                </Link>
            </Box>
        );
    }

    return (
        <Box p="2rem">
            <Heading mb={4}>{event.name}</Heading>
            <Text mb={2}><strong>Date:</strong> {new Date(event.date).toLocaleDateString()}</Text>
            <Text mb={2}><strong>Start time:</strong> {new Date(event.date).toLocaleTimeString()}</Text>
            <Text mb={2}><strong>Available spots:</strong> {event.availableSpots}</Text>
            <Text mb={6}>{event.description}</Text>

            {/* Counter for adults */}
            <HStack mb={6}>
                <Button onClick={() => setAdults(Math.max(0, adults - 1))} disabled={adults === 0}>
                    -
                </Button>
                <Text>{adults} adult{adults !== 1 ? "s" : ""}</Text>
                <Button
                    onClick={() => setAdults(adults + 1)}
                    disabled={adults >= event.availableSpots}
                >
                    +
                </Button>
            </HStack>

            <Button
                colorScheme="blue"
                isDisabled={adults === 0}
                onClick={() => alert(`Reserved ${adults} spots for ${event.name}`)}
            >
                Reserve
            </Button>

            <Box mt={4}>
                <Link as={RouterLink} to="/explore" color="blue.500">
                    ← Back to Explore
                </Link>
            </Box>
        </Box>
    );
}
