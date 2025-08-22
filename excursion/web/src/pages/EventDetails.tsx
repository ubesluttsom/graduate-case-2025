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
    Alert,
    AlertIcon,
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
const CANCELLATION_FEE = 25.00; // $25 cancellation fee

export default function EventDetails() {
    const { id } = useParams<{ id: string }>();
    const [searchParams] = useSearchParams();
    const guestId = searchParams.get("guestId");
    const toast = useToast();

    const [event, setEvent] = useState<Event | null>(null);
    const [loading, setLoading] = useState(true);
    const [adults, setAdults] = useState(0);

    const { isOpen, onOpen, onClose } = useDisclosure();
    const { 
        isOpen: isCancelModalOpen, 
        onOpen: onCancelModalOpen, 
        onClose: onCancelModalClose 
    } = useDisclosure();

    // Room guests
    const [roomGuests, setRoomGuests] = useState<Guest[]>([]);
    const [selectedGuests, setSelectedGuests] = useState<string[]>([]);
    const [guestsToCancel, setGuestsToCancel] = useState<string[]>([]);

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

    // Helper: unbook guests from event
    async function unbookGuests(guestIds: string[]) {
        try {
            // Remove each guest from the event
            for (const gId of guestIds) {
                await fetch(
                    `${API_BASE}/events/${event!.id}/guests/${gId}`,
                    { method: "DELETE" }
                );
            }

            // Create cancellation transaction for each guest
            for (const gId of guestIds) {
                const guest = roomGuests.find(g => g.id === gId);
                if (guest?.roomId) {
                    await fetch(`${API_BASE}/transactions`, {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                        },
                        body: JSON.stringify({
                            amount: CANCELLATION_FEE,
                            description: `Cancellation fee - ${event!.name}`,
                            roomId: guest.roomId,
                            guestId: gId,
                            transactionDate: new Date().toISOString(),
                        }),
                    });
                }
            }

            // Reload event to update spots
            await loadEvent(event!.id);

            toast({
                status: "success",
                title: "Successfully unbooked",
                description: `Removed ${guestIds.length} guest${guestIds.length > 1 ? 's' : ''} from the event. Cancellation fee of $${CANCELLATION_FEE.toFixed(2)} per person has been added to your room tab.`,
                duration: 6000,
            });
        } catch (err) {
            console.error("Failed to unbook guests:", err);
            toast({
                status: "error",
                title: "Unbooking failed",
            });
        }
    }

    const handleCancelBooking = (guestIds: string[]) => {
        setGuestsToCancel(guestIds);
        onCancelModalOpen();
    };

    const confirmCancellation = async () => {
        await unbookGuests(guestsToCancel);
        onCancelModalClose();
        setGuestsToCancel([]);
    };

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

    const currentGuestSignedUp = useMemo(() => {
        if (!event || !guestId) return false;
        return event.guestIds?.includes(guestId) ?? false;
    }, [event, guestId]);

    const signedUpRoommates = useMemo(() => {
        if (!event) return [];
        return roomGuests.filter(guest => 
            guest.id !== guestId && event.guestIds?.includes(guest.id)
        );
    }, [event, roomGuests, guestId]);

    const handleToggleGuest = (gId: string) => {
        setSelectedGuests((prev) =>
            prev.includes(gId) ? prev.filter((id) => id !== gId) : [...prev, gId]
        );
    };

    if (loading) {
        return (
            <Box p={{ base: "1rem", sm: "1.25rem", md: "2rem" }} textAlign="center" bg="var(--navy)" minH="100vh">
                <Spinner size="lg" />
            </Box>
        );
    }

    if (!event) {
        return (
            <Box p={{ base: "1rem", sm: "1.25rem", md: "2rem" }} bg="var(--navy)" minH="100vh">
                <Heading size="md" color="white" mb={4}>Event not found</Heading>
                <Link
                    as={RouterLink}
                    to={`/explore${guestId ? `?guestId=${guestId}` : ""}`}
                    color="blue.300"
                >
                    Back to Explore
                </Link>
            </Box>
        );
    }

    const totalCancellationFee = guestsToCancel.length * CANCELLATION_FEE;
    const guestNamesToCancel = roomGuests
        .filter(g => guestsToCancel.includes(g.id))
        .map(g => `${g.firstName} ${g.lastName}`)
        .join(', ');

    return (
        <Box
            p={{ base: "0", md: "0" }}
            className="explore"
            maxW="100%"
            minH="100vh"
            bg="var(--navy)"
        >
            {/* Top header */}
            <Box
                as="header"
                className="explore-header"
                position="sticky"
                top={0}
                zIndex={10}
                px={{ base: "1rem", sm: "1.25rem", md: "2rem" }}
                py={{ base: "0.75rem", sm: "0.9rem" }}
                display="flex"
                alignItems="center"
                justifyContent="space-between"
                bg="rgba(15,35,58,0.9)"
                backdropFilter="saturate(120%) blur(6px)"
                borderBottom="1px solid rgba(255,255,255,0.06)"
            >
                <Button
                    as={RouterLink}
                    to={`/explore${guestId ? `?guestId=${guestId}` : ""}`}
                    aria-label="Back to Explore"
                    variant="ghost"
                    size="sm"
                    className="back-btn"
                    _hover={{ bg: "rgba(255,255,255,0.08)" }}
                    _active={{ transform: "translateY(1px)" }}
                    color="#ffdcb5"
                >
                    ←
                </Button>

                <HStack spacing={2}>
                    <Box aria-hidden="true" fontSize="lg" transform="translateY(-1px)">
                        ⚓
                    </Box>
                    <Box lineHeight="1">
                        <Text fontSize="xs" letterSpacing=".12em" fontWeight="700" color="#ffdcb5">
                            EXPLORE
                        </Text>
                        <Text fontSize="xs" letterSpacing=".06em" color="#ffdcb5" opacity={0.9}>
                            Event Details
                        </Text>
                    </Box>
                </HStack>

                {/* right spacer to balance layout */}
                <Box w="32px" />
            </Box>

            {/* Page content */}
            <Box px={{ base: "1rem", sm: "1.25rem", md: "2rem" }} pt={{ base: 4, md: 6 }} pb={{ base: 6, md: 10 }}>

            {/* Event Card */}
            <Box
                borderRadius="lg"
                boxShadow="md"
                bg="var(--navy-700)"
                color="white"
                overflow="hidden"
                className="event-card"
                mb={6}
            >

                {/* Image section */}
                <Box position="relative" height={{ base: "200px", sm: "240px" }} bg="gray.800">
                    {event.imageUrl && (
                        <Box
                            as="img"
                            src={event.imageUrl}
                            alt={event.name}
                            width="100%"
                            height="100%"
                            objectFit="cover"
                            onError={(e) => {
                                e.currentTarget.style.display = 'none';
                            }}
                        />
                    )}
                    <Box
                        position="absolute"
                        top={0}
                        left={0}
                        right={0}
                        bottom={0}
                        bgGradient={
                            event.imageUrl
                                ? "linear(to-b, transparent 0%, blackAlpha.300 50%, blackAlpha.600 100%)"
                                : "linear(to-br, blue.400, purple.500, pink.400)"
                        }
                    />
                    <Box position="absolute" bottom={4} left={4} right={4}>
                        <Heading size={{ base: "lg", sm: "xl" }} color="white" textShadow="0 1px 3px rgba(0,0,0,0.8)">
                            {event.name}
                        </Heading>
                    </Box>
                    
                    {/* Status indicators */}
                    {currentGuestSignedUp && (
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
                            boxShadow="0 2px 4px rgba(0,0,0,0.3)"
                            className="status-badge"
                        >
                            ✓ You're signed up
                        </Box>
                    )}
                </Box>

                {/* Content */}
                <VStack align="stretch" spacing={4} p={{ base: 4, sm: 5 }}>
                    <Text fontSize={{ base: "sm", sm: "md" }} color="gray.200">
                        {event.description}
                    </Text>

                    <HStack justify="space-between" fontSize={{ base: "sm", sm: "md" }}>
                        <VStack align="start" spacing={1}>
                            <Text>
                                <strong>Date:</strong> {new Date(event.date).toLocaleDateString()}
                            </Text>
                            <Text>
                                <strong>Time:</strong> {new Date(event.date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                            </Text>
                        </VStack>
                        <VStack align="end" spacing={1}>
                            <Text
                                color={remainingSpots > 0 ? "green.300" : "red.300"}
                                fontWeight="medium"
                                className={remainingSpots > 0 ? "spots-left" : "spots-full"}
                            >
                                {remainingSpots > 0 ? `${remainingSpots} spots left` : "Fully booked"}
                            </Text>
                            <Text fontSize="xs" color="gray.400">
                                {event.guestIds?.length ?? 0} of {event.availableSpots} taken
                            </Text>
                            {event.price && (
                                <Text fontWeight="medium" color="orange.200">
                                    ${event.price.toFixed(2)}
                                </Text>
                            )}
                        </VStack>
                    </HStack>

                    {/* Roommate signup status */}
                    {signedUpRoommates.length > 0 && (
                        <Box bg="blue.900" p={3} borderRadius="md" borderLeft="4px solid" borderColor="blue.400">
                            <Text fontSize="sm" color="blue.200">
                                👥 <strong>Roommates signed up:</strong> {signedUpRoommates.map(r => r.firstName).join(', ')}
                            </Text>
                        </Box>
                    )}

                    {/* Current user status */}
                    {currentGuestSignedUp && (
                        <Box bg="green.900" p={3} borderRadius="md" borderLeft="4px solid" borderColor="green.400">
                            <Text fontSize="sm" color="green.200">
                                ✅ <strong>You are signed up for this event!</strong>
                            </Text>
                        </Box>
                    )}
                </VStack>
            </Box>

            {/* Who will join section */}
            <Box
                bg="var(--navy-700)"
                borderRadius="lg"
                p={{ base: 4, sm: 5 }}
                mb={6}
            >
                <Heading size="sm" mb={4} color="white" fontSize={{ base: "md", sm: "lg" }}>
                    {currentGuestSignedUp ? "Manage your booking" : "Who will join?"}
                </Heading>
                
                {roomGuests.length === 0 ? (
                    <Text color="gray.400" fontSize="sm">
                        No room information available
                    </Text>
                ) : (
                    <VStack align="start" spacing={3}>
                        {roomGuests.map((g) => {
                            const isSignedUp = event.guestIds?.includes(g.id);
                            const isSelected = selectedGuests.includes(g.id);
                            
                            return (
                                <HStack key={g.id} spacing={3} w="100%">
                                    <Box
                                        as="input"
                                        type="checkbox"
                                        checked={isSelected}
                                        onChange={() => handleToggleGuest(g.id)}
                                        w="18px"
                                        h="18px"
                                        bg="var(--navy)"
                                        border="2px solid"
                                        borderColor="var(--orange-600)"
                                        borderRadius="4px"
                                        cursor="pointer"
                                        _checked={{
                                            bg: "var(--orange)",
                                            borderColor: "var(--orange)",
                                        }}
                                        sx={{
                                            "&:checked": {
                                                background: "var(--orange)",
                                                borderColor: "var(--orange)",
                                            }
                                        }}
                                    />
                                    <HStack flex="1" justify="space-between">
                                        <Text color="white" fontSize={{ base: "sm", sm: "md" }}>
                                            {g.firstName} {g.lastName}
                                        </Text>
                                        {isSignedUp && (
                                            <Text fontSize="xs" color="green.300" fontWeight="bold">
                                                ✓ Signed up
                                            </Text>
                                        )}
                                    </HStack>
                                </HStack>
                            );
                        })}
                    </VStack>
                )}
            </Box>

            {/* Cancellation fee notice for signed up users */}
            {currentGuestSignedUp && (
                <Box
                    bg="orange.900"
                    p={3}
                    borderRadius="md"
                    borderLeft="4px solid"
                    borderColor="orange.400"
                    mb={6}
                >
                    <Text fontSize="sm" color="orange.200">
                        ⚠️ <strong>Cancellation Policy:</strong> A cancellation fee of ${CANCELLATION_FEE.toFixed(2)} per person will be charged to your room tab for unbooking.
                    </Text>
                </Box>
            )}

            {/* Action buttons */}
            <VStack spacing={3} w="100%">
                {/* Show booking button if there are unbooked guests and spots available */}
                {roomGuests.some(g => !event.guestIds?.includes(g.id)) && remainingSpots > 0 && (
                    <Button
                        className="btn-cta"
                        w="100%"
                        size={{ base: "md", sm: "lg" }}
                        isDisabled={
                            selectedGuests.length === 0 || 
                            !selectedGuests.some(gId => !event.guestIds?.includes(gId))
                        }
                        onClick={onOpen}
                        bg="var(--orange)"
                        color="#1b1b1b"
                        borderRadius="var(--radius)"
                        fontWeight="700"
                        _hover={{
                            bg: "var(--orange-600)"
                        }}
                        _disabled={{
                            bg: "gray.600",
                            color: "gray.400"
                        }}
                    >
                        Sign Up Selected
                    </Button>
                )}

                {/* Show unbook buttons if there are booked guests */}
                {roomGuests.some(g => event.guestIds?.includes(g.id)) && (
                    <>
                        <Button
                            w="100%"
                            size={{ base: "md", sm: "lg" }}
                            onClick={() => {
                                const signedUpGuests = roomGuests
                                    .filter(g => event.guestIds?.includes(g.id))
                                    .map(g => g.id);
                                if (signedUpGuests.length > 0) {
                                    handleCancelBooking(signedUpGuests);
                                }
                            }}
                            bg="red.600"
                            color="white"
                            borderRadius="var(--radius)"
                            fontWeight="700"
                            _hover={{
                                bg: "red.700"
                            }}
                        >
                            Unbook All Signed Up Guests
                        </Button>
                        
                        <Button
                            w="100%"
                            size={{ base: "md", sm: "lg" }}
                            isDisabled={
                                selectedGuests.length === 0 || 
                                !selectedGuests.some(gId => event.guestIds?.includes(gId))
                            }
                            onClick={() => {
                                const selectedSignedUpGuests = selectedGuests.filter(gId => 
                                    event.guestIds?.includes(gId)
                                );
                                if (selectedSignedUpGuests.length > 0) {
                                    handleCancelBooking(selectedSignedUpGuests);
                                }
                            }}
                            bg="orange.600"
                            color="white"
                            borderRadius="var(--radius)"
                            fontWeight="700"
                            _hover={{
                                bg: "orange.700"
                            }}
                            _disabled={{
                                bg: "gray.600",
                                color: "gray.400"
                            }}
                        >
                            Unbook Selected
                        </Button>
                    </>
                )}

                {/* Show message if fully booked and no one from room is signed up */}
                {remainingSpots === 0 && !roomGuests.some(g => event.guestIds?.includes(g.id)) && (
                    <Box
                        w="100%"
                        p={4}
                        bg="red.900"
                        borderRadius="var(--radius)"
                        textAlign="center"
                    >
                        <Text color="red.200" fontWeight="medium">
                            Event is fully booked
                        </Text>
                    </Box>
                )}

                <Button
                    as={RouterLink}
                    to={`/explore${guestId ? `?guestId=${guestId}` : ""}`}
                    variant="outline"
                    w="100%"
                    size={{ base: "md", sm: "lg" }}
                    className="btn-outline"
                    borderColor="var(--orange-600)"
                    color="#ffdcb5"
                    _hover={{
                        bg: "var(--navy-700)"
                    }}
                >
                    Back to Activities
                </Button>
            </VStack>
            </Box>

            {/* Reservation Modal */}
            <Modal isOpen={isOpen} onClose={onClose} isCentered>
                <ModalOverlay />
                <ModalContent mx={4} bg="var(--navy-700)" color="white">
                    <ModalHeader>Confirm Reservation</ModalHeader>
                    <ModalBody>
                        <Text mb={2}>
                            <strong>Activity:</strong> {event.name}
                        </Text>
                        <Text>
                            <strong>Guests to sign up:</strong>{" "}
                            {roomGuests
                                .filter((g) => selectedGuests.includes(g.id) && !event.guestIds?.includes(g.id))
                                .map((g) => `${g.firstName} ${g.lastName}`)
                                .join(", ")}
                        </Text>
                    </ModalBody>
                    <ModalFooter>
                        <Button
                            className="btn-cta"
                            mr={3}
                            onClick={async () => {
                                try {
                                    // Reserve each selected guest who isn't already signed up
                                    const guestsToBook = selectedGuests.filter(gId => 
                                        !event.guestIds?.includes(gId)
                                    );
                                    
                                    for (const gId of guestsToBook) {
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
                                        description: `Signed up ${guestsToBook.length} guest${guestsToBook.length > 1 ? 's' : ''}`,
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

                        <Button variant="ghost" onClick={onClose} color="white">
                            Cancel
                        </Button>
                    </ModalFooter>
                </ModalContent>
            </Modal>

            {/* Cancellation Confirmation Modal */}
            <Modal isOpen={isCancelModalOpen} onClose={onCancelModalClose} isCentered>
                <ModalOverlay />
                <ModalContent mx={4} bg="var(--navy-700)" color="white">
                    <ModalHeader color="red.300">Confirm Cancellation</ModalHeader>
                    <ModalBody>
                        <Alert status="warning" mb={4} bg="orange.900" borderRadius="md">
                            <AlertIcon color="orange.300" />
                            <Box>
                                <Text fontWeight="bold" color="orange.200">
                                    Cancellation Fee Notice
                                </Text>
                                <Text fontSize="sm" color="orange.100">
                                    A fee of ${CANCELLATION_FEE.toFixed(2)} per person will be charged to your room tab.
                                </Text>
                            </Box>
                        </Alert>
                        
                        <VStack align="start" spacing={2}>
                            <Text>
                                <strong>Activity:</strong> {event.name}
                            </Text>
                            <Text>
                                <strong>Cancelling for:</strong> {guestNamesToCancel}
                            </Text>
                            <Text>
                                <strong>Number of guests:</strong> {guestsToCancel.length}
                            </Text>
                            <Text fontWeight="bold" color="orange.300">
                                <strong>Total cancellation fee:</strong> ${totalCancellationFee.toFixed(2)}
                            </Text>
                        </VStack>
                    </ModalBody>
                    <ModalFooter>
                        <Button
                            mr={3}
                            onClick={confirmCancellation}
                            bg="red.600"
                            color="white"
                            _hover={{ bg: "red.700" }}
                        >
                            Confirm Cancellation
                        </Button>

                        <Button variant="ghost" onClick={onCancelModalClose} color="white">
                            Keep Booking
                        </Button>
                    </ModalFooter>
                </ModalContent>
            </Modal>
        </Box>
    );
}
