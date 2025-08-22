import { useEffect, useState } from "react";
import {
    Modal,
    ModalOverlay,
    ModalContent,
    ModalHeader,
    ModalBody,
    ModalFooter,
    ModalCloseButton,
    Button,
    VStack,
    HStack,
    Text,
    Divider,
    Spinner,
    Alert,
    AlertIcon,
    Box,
} from "@chakra-ui/react";

type Transaction = {
    id: string;
    amount: number;
    description: string;
    roomId: string;
    guestId: string;
    transactionDate: string;
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

type RoomSummaryProps = {
    isOpen: boolean;
    onClose: () => void;
    guest: Guest | null;
};

const API_BASE = "http://localhost:7071/api";

export default function RoomSummary({ isOpen, onClose, guest }: RoomSummaryProps) {
    const [transactions, setTransactions] = useState<Transaction[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!isOpen || !guest?.roomId) return;

        async function fetchTransactions() {
            try {
                setLoading(true);
                setError(null);
                const res = await fetch(`${API_BASE}/rooms/${guest.roomId}/transactions`);
                
                if (!res.ok) {
                    throw new Error(`HTTP ${res.status}`);
                }
                
                const data: Transaction[] = await res.json();
                setTransactions(data ?? []);
            } catch (err: any) {
                console.error("Failed to fetch transactions:", err);
                setError(err?.message ?? "Failed to fetch transactions");
            } finally {
                setLoading(false);
            }
        }

        fetchTransactions();
    }, [isOpen, guest?.roomId]);

    const totalAmount = transactions.reduce((sum, transaction) => sum + transaction.amount, 0);

    return (
        <Modal isOpen={isOpen} onClose={onClose} size="lg" isCentered>
            <ModalOverlay />
            <ModalContent mx={4}>
                <ModalHeader>
                    Room Tab Summary
                    {guest && (
                        <Text fontSize="sm" fontWeight="normal" color="gray.600">
                            {guest.firstName} {guest.lastName}
                        </Text>
                    )}
                </ModalHeader>
                <ModalCloseButton />
                
                <ModalBody>
                    {loading && (
                        <Box textAlign="center" py={4}>
                            <Spinner size="lg" />
                        </Box>
                    )}

                    {error && (
                        <Alert status="error" mb={4}>
                            <AlertIcon />
                            {error}
                        </Alert>
                    )}

                    {!loading && !error && (
                        <>
                            {transactions.length === 0 ? (
                                <Text color="gray.500" textAlign="center" py={4}>
                                    No transactions found for this room.
                                </Text>
                            ) : (
                                <VStack spacing={3} align="stretch">
                                    {transactions.map((transaction) => (
                                        <Box key={transaction.id} p={3} borderWidth="1px" borderRadius="md">
                                            <HStack justify="space-between">
                                                <VStack align="start" spacing={1}>
                                                    <Text fontWeight="medium">
                                                        {transaction.description}
                                                    </Text>
                                                    <Text fontSize="sm" color="gray.600">
                                                        {new Date(transaction.transactionDate).toLocaleDateString()} at{" "}
                                                        {new Date(transaction.transactionDate).toLocaleTimeString()}
                                                    </Text>
                                                </VStack>
                                                <Text fontWeight="bold" color={transaction.amount >= 0 ? "green.600" : "red.600"}>
                                                    ${transaction.amount.toFixed(2)}
                                                </Text>
                                            </HStack>
                                        </Box>
                                    ))}
                                    
                                    <Divider />
                                    
                                    <HStack justify="space-between" p={3} bg="gray.50" borderRadius="md">
                                        <Text fontSize="lg" fontWeight="bold">
                                            Total:
                                        </Text>
                                        <Text fontSize="lg" fontWeight="bold" color={totalAmount >= 0 ? "green.600" : "red.600"}>
                                            ${totalAmount.toFixed(2)}
                                        </Text>
                                    </HStack>
                                </VStack>
                            )}
                        </>
                    )}
                </ModalBody>

                <ModalFooter>
                    <Button onClick={onClose}>Close</Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    );
}
