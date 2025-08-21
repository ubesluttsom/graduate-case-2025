import { useEffect, useState } from "react";
import { useParams, Link as RouterLink, useSearchParams } from "react-router-dom";
import {
    Box,
    Heading,
    Text,
    Spinner,
    Button,
    HStack,
    Link,
    useDisclosure,
    Modal,
    ModalOverlay,
    ModalContent,
    ModalHeader,
    ModalBody,
    ModalFooter
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
    const [searchParams] = useSearchParams();
    const guestId = searchParams.get("guestId"); // 👈 get guestId from URL

    const [event, setEvent] = useState<Event | null>(null);
    const [loading, setLoading] = useState(true);
    const [adults, setAdults] = useState(0);

    const { isOpen, onOpen, onClose } = useDisclosure();

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
                <Link as={RouterLink} to={`/explore${guestId ? `?guestId=${guestId}` : ""}`} color="blue.500">
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
                onClick={onOpen}
            >
                Reserve
            </Button>

            <Box mt={4}>
                {/* 👇 preserve guestId when going back */}
                <Link as={RouterLink} to={`/explore${guestId ? `?guestId=${guestId}` : ""}`} color="blue.500">
                    ← Back to Explore
                </Link>
            </Box>

            {/* Reservation Modal */}
            <Modal isOpen={isOpen} onClose={onClose} isCentered>
                <ModalOverlay />
                <ModalContent mx={4}>
                    <ModalHeader>Confirm Reservation</ModalHeader>
                    <ModalBody>
                        <Text mb={2}><strong>Activity:</strong> {event.name}</Text>
                        <Text><strong>People:</strong> {adults}</Text>
                        {guestId && <Text><strong>GuestId:</strong> {guestId}</Text>}
                    </ModalBody>
                    <ModalFooter>
                        <Button
                            colorScheme="blue"
                            mr={3}
                            onClick={() => {
                                // TODO: send reservation with guestId
                                console.log(`Reserved ${adults} spots for ${event.name} by guest ${guestId}`);
                                onClose();
                            }}
                        >
                            Confirm
                        </Button>
                        <Button variant="ghost" onClick={onClose}>
                            Cancel
                        </Button>
                    </ModalFooter>
                </ModalContent>
            </Modal>
        </Box>
    );
}
