import { useEffect, useMemo, useState } from "react";
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
    ModalFooter,
    Checkbox,
    VStack,
    useToast,
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

type Guest = {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    roomId?: string;
};

const API_BASE = "http://localhost:7071/api";

export default function EventDetails() {
    const { id } = useParams<{ id: string }>();
    const [searchParams] = useSearchParams();
    const guestId = searchParams.get("guestId");
    const toast = useToast();

    const [event, setEvent] = useState<Event | null>(null);
    const [loading, setLoading] = useState(true);
    const [adults, setAdults] = useState(0);

    const { isOpen, onOpen, onClose } = useDisclosure();

    // Room guests
    const [roomGuests, setRoomGuests] = useState<Guest[]>([]);
    const [selectedGuests, setSelectedGuests] = useState<string[]>([]);

    // Helper: fetch one event
    async function loadEvent(eventId: string) {
        const res = await fetch(`${API_BASE}/events/${eventId}`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const data: Event = await res.json();
        setEvent(data);
    }

    // Helper: fetch room members
    async function loadRoomGuests(activeGuestId: string) {
        try {
            // 1. Fetch active guest
            const guestRes = await fetch(`${API_BASE}/guests/${activeGuestId}`);
            if (!guestRes.ok) throw new Error(`HTTP ${guestRes.status}`);
            const guest: Guest = await guestRes.json();

            if (!guest.roomId) return;

            // 2. Fetch room
            const roomRes = await fetch(`${API_BASE}/rooms/${guest.roomId}`);
            if (!roomRes.ok) throw new Error(`HTTP ${roomRes.status}`);
            const roomData = await roomRes.json();

            if (!roomData.guestIds) return;

            // 3. Fetch each guest in that room
            const guests: Guest[] = await Promise.all(
                roomData.guestIds.map(async (gId: string) => {
                    const gRes = await fetch(`${API_BASE}/guests/${gId}`);
                    if (!gRes.ok) return null;
                    return await gRes.json();
                })
            ).then((res) => res.filter((g): g is Guest => g !== null));

            setRoomGuests(guests);
            // By default, select the active guest only
            setSelectedGuests([activeGuestId]);
        } catch (err) {
            console.error("Failed to fetch room guests:", err);
        }
    }

    useEffect(() => {
        if (!id) return;
        setLoading(true);
        loadEvent(id)
            .catch((err) => console.error("Failed to fetch event:", err))
            .finally(() => setLoading(false));
    }, [id]);

    useEffect(() => {
        if (guestId) {
            loadRoomGuests(guestId);
        }
    }, [guestId]);

    const remainingSpots = useMemo(() => {
        if (!event) return 0;
        const taken = Array.isArray(event.guestIds) ? event.guestIds.length : 0;
        return Math.max(0, event.availableSpots - taken);
    }, [event]);

    const handleToggleGuest = (gId: string) => {
        setSelectedGuests((prev) =>
            prev.includes(gId) ? prev.filter((id) => id !== gId) : [...prev, gId]
        );
    };

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
                <Link
                    as={RouterLink}
                    to={`/explore${guestId ? `?guestId=${guestId}` : ""}`}
                    color="blue.500"
                >
                    Back to Explore
                </Link>
            </Box>
        );
    }

    return (
        <Box p="2rem">
            <Heading mb={4}>{event.name}</Heading>
            <Text mb={2}>
                <strong>Date:</strong> {new Date(event.date).toLocaleDateString()}
            </Text>
            <Text mb={2}>
                <strong>Start time:</strong> {new Date(event.date).toLocaleTimeString()}
            </Text>
            <Text mb={2}>
                <strong>Available spots:</strong> {remainingSpots}
            </Text>
            <Text mb={6}>{event.description}</Text>

            {/* Guests in same room */}
            <Heading size="sm" mb={2}>Who will join?</Heading>
            <VStack align="start" mb={6}>
                {roomGuests.map((g) => (
                    <Checkbox
                        key={g.id}
                        isChecked={selectedGuests.includes(g.id)}
                        onChange={() => handleToggleGuest(g.id)}
                    >
                        {g.firstName} {g.lastName}
                    </Checkbox>
                ))}
            </VStack>

            <Button
                colorScheme="blue"
                isDisabled={selectedGuests.length === 0 || remainingSpots === 0}
                onClick={onOpen}
            >
                Reserve
            </Button>

            <Box mt={4}>
                <Link
                    as={RouterLink}
                    to={`/explore${guestId ? `?guestId=${guestId}` : ""}`}
                    color="blue.500"
                >
                    ← Back to Explore
                </Link>
            </Box>

            {/* Reservation Modal */}
            <Modal isOpen={isOpen} onClose={onClose} isCentered>
                <ModalOverlay />
                <ModalContent mx={4}>
                    <ModalHeader>Confirm Reservation</ModalHeader>
                    <ModalBody>
                        <Text mb={2}>
                            <strong>Activity:</strong> {event.name}
                        </Text>
                        <Text>
                            <strong>Guests:</strong>{" "}
                            {roomGuests
                                .filter((g) => selectedGuests.includes(g.id))
                                .map((g) => `${g.firstName} ${g.lastName}`)
                                .join(", ")}
                        </Text>
                    </ModalBody>
                    <ModalFooter>
                        <Button
                            colorScheme="blue"
                            mr={3}
                            onClick={async () => {
                                try {
                                    // Reserve each selected guest
                                    for (const gId of selectedGuests) {
                                        await fetch(
                                            `${API_BASE}/events/${event.id}/guests/${gId}`,
                                            { method: "POST" }
                                        );
                                    }

                                    // Reload event to update spots
                                    await loadEvent(event.id);

                                    toast({
                                        status: "success",
                                        title: "Reservation confirmed",
                                    });
                                    onClose();
                                } catch (err) {
                                    console.error("Failed to reserve spots:", err);
                                    toast({
                                        status: "error",
                                        title: "Reservation failed",
                                    });
                                }
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