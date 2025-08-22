import { useEffect, useState } from "react";
import { useLocation, Link as RouterLink, useSearchParams, useNavigate } from "react-router-dom";
import {
    Box,
    Heading,
    Text,
    Spinner,
    Alert,
    AlertIcon,
    Button,
    HStack,
    VStack,
    useDisclosure,
    SimpleGrid,
} from "@chakra-ui/react";
import RoomSummary from "../components/RoomSummary";
import "./explore.css";

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
    const navigate = useNavigate();

    const [searchParams] = useSearchParams();
    const guestId = searchParams.get("guestId") ?? "";

    const [guest, setGuest] = useState<Guest | null>(null);
    const [roommates, setRoommates] = useState<Guest[]>([]);
    const [events, setEvents] = useState<Event[]>([]);
    const [eventsLoading, setEventsLoading] = useState(true);
    const [eventsError, setEventsError] = useState<string | null>(null);

    const { isOpen: isRoomSummaryOpen, onOpen: onRoomSummaryOpen, onClose: onRoomSummaryClose } =
        useDisclosure();

    // Fetch guest and roommates
    useEffect(() => {
        if (!guestId) return;

        async function fetchGuestAndRoommates() {
            try {
                const guestRes = await fetch(`${API_BASE}/guests/${guestId}`);
                if (!guestRes.ok) throw new Error(`HTTP ${guestRes.status}`);
                const guestData: Guest = await guestRes.json();
                setGuest(guestData);

                if (guestData.roomId) {
                    const roomRes = await fetch(`${API_BASE}/rooms/${guestData.roomId}`);
                    if (roomRes.ok) {
                        const roomData = await roomRes.json();

                        if (roomData.guestIds && roomData.guestIds.length > 0) {
                            const roommatePromises = roomData.guestIds
                                .filter((id: string) => id !== guestId)
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

    // Fetch events
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
            <Box p={{ base: "1rem", md: "2rem" }} textAlign="center" bg="var(--navy)" minH="100vh">
                <Spinner size="lg" />
            </Box>
        );
    }

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
                    onClick={() => navigate("/")}
                    aria-label="Back to Home"
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
                            Polar Expeditions
                        </Text>
                    </Box>
                </HStack>

                {/* right spacer to balance layout */}
                <Box w="32px" />
            </Box>

            {/* Page content */}
            <Box px={{ base: "1rem", sm: "1.25rem", md: "2rem" }} pt={{ base: 4, md: 6 }} pb={{ base: 6, md: 10 }}>
                <HStack justify="space-between" align="start" mb={{ base: 4, md: 6 }}>
                    <Heading fontSize={{ base: "xl", sm: "2xl" }} color="white">
                        {guest
                            ? `Hi, ${guest.firstName} ${guest.lastName}! 👋`
                            : fallbackName
                                ? `Hi, ${fallbackName}! 👋`
                                : "Hi there 👋"}
                    </Heading>

                    {guest?.roomId && (
                        <Button
                            colorScheme="blue"
                            variant="outline"
                            onClick={onRoomSummaryOpen}
                            className="header-outline-btn"
                            size="sm"
                        >
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

                <Heading
                    size="md"
                    mb={4}
                    className="section-title"
                    fontSize={{ base: "md", sm: "lg" }}
                    color="white"
                >
                    Available Activities
                </Heading>

                {events.length === 0 ? (
                    <Text color="white">No activities found.</Text>
                ) : (
                    <SimpleGrid columns={{ base: 1, md: 2, xl: 3 }} spacing={{ base: 3, md: 4 }}>
                        {events.map((event) => {
                            const search = guestId ? `?guestId=${guestId}` : "";
                            const totalSignedUp = event.guestIds?.length ?? 0;
                            const remainingSpots = event.availableSpots - totalSignedUp;

                            const isCurrentGuestSignedUp = guest && event.guestIds?.includes(guest.id);
                            const signedUpRoommates = roommates.filter((roommate) =>
                                event.guestIds?.includes(roommate.id)
                            );

                            return (
                                <Box
                                    key={event.id}
                                    borderRadius="lg"
                                    boxShadow="md"
                                    bg="var(--navy-700)"
                                    color="white"
                                    position="relative"
                                    overflow="hidden"
                                    className="event-card"
                                    display="flex"
                                    flexDirection="column"
                                >
                                    {/* Image */}
                                    <Box position="relative" height={{ base: "150px", sm: "180px" }} bg="gray.800">
                                        {event.imageUrl && (
                                            <Box
                                                as="img"
                                                src={event.imageUrl}
                                                alt={event.name}
                                                width="100%"
                                                height="100%"
                                                objectFit="cover"
                                                onError={(e) => {
                                                    e.currentTarget.style.display = "none";
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
                                        <Box position="absolute" bottom={3} left={{ base: 3, sm: 4 }} right={{ base: 3, sm: 4 }}>
                                            <Heading
                                                size="sm"
                                                fontSize={{ base: "md", sm: "lg" }}
                                                color="white"
                                                textShadow="0 1px 3px rgba(0,0,0,0.8)"
                                            >
                                                {event.name}
                                            </Heading>
                                        </Box>
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
                                                boxShadow="0 2px 4px rgba(0,0,0,0.3)"
                                                className="status-badge"
                                            >
                                                ✓ Signed up
                                            </Box>
                                        )}
                                    </Box>

                                    {/* Content */}
                                    <VStack align="stretch" spacing={3} p={{ base: 3, sm: 4 }} flex="1">
                                        <Box>
                                            <Text fontSize={{ base: "sm", sm: "md" }} color="gray.200" mb={2}>
                                                {event.description}
                                            </Text>
                                        </Box>

                                        <HStack justify="space-between" fontSize={{ base: "sm", sm: "md" }} align="start">
                                            <VStack align="start" spacing={1}>
                                                <Text>
                                                    <strong>Date:</strong> {new Date(event.date).toLocaleDateString()}
                                                </Text>
                                                <Text>
                                                    <strong>Time:</strong>{" "}
                                                    {new Date(event.date).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
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
                                                    {totalSignedUp} of {event.availableSpots} taken
                                                </Text>
                                                {event.price && <Text fontWeight="medium">${event.price.toFixed(2)}</Text>}
                                            </VStack>
                                        </HStack>

                                        {signedUpRoommates.length > 0 && (
                                            <Text fontSize={{ base: "sm", sm: "md" }} color="blue.300">
                                                👥 {signedUpRoommates.map((r) => r.firstName).join(", ")}{" "}
                                                {signedUpRoommates.length === 1 ? "is" : "are"} signed up
                                            </Text>
                                        )}

                                        <HStack justify="flex-end" align="center" mt="auto">
                                            {isCurrentGuestSignedUp ? (
                                                <Button
                                                    as={RouterLink}
                                                    to={`/events/${event.id}${search}`}
                                                    size="sm"
                                                    colorScheme="blue"
                                                    variant="solid"
                                                >
                                                    View Event
                                                </Button>
                                            ) : remainingSpots > 0 ? (
                                                <Button
                                                    as={RouterLink}
                                                    to={`/events/${event.id}${search}`}
                                                    size="sm"
                                                    colorScheme="green"
                                                    className="btn-cta"
                                                >
                                                    Sign Up
                                                </Button>
                                            ) : (
                                                <Text fontSize="sm" color="red.300" fontWeight="medium">
                                                    Fully booked
                                                </Text>
                                            )}
                                        </HStack>
                                    </VStack>
                                </Box>
                            );
                        })}
                    </SimpleGrid>
                )}
            </Box>

            <RoomSummary isOpen={isRoomSummaryOpen} onClose={onRoomSummaryClose} guest={guest} />
        </Box>
    );
}
