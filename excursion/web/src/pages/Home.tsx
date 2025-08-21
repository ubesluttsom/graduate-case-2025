import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function Home() {
    const [guests, setGuests] = useState<any[]>([]);
    const [selectedGuestId, setSelectedGuestId] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        fetch("http://localhost:7071/api/guests")
            .then((res) => res.json())
            .then(setGuests)
            .catch((err) => console.error("Failed to load guests", err));
    }, []);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedGuestId) return;
        // Navigate to explore, passing guestId in URL
        navigate(`/explore?guestId=${selectedGuestId}`);
    };

    return (
        <div style={{ padding: "2rem" }}>
            <h1>Select a guest</h1>
            <form onSubmit={handleSubmit}>
                <select
                    value={selectedGuestId}
                    onChange={(e) => setSelectedGuestId(e.target.value)}
                >
                    <option value="">-- choose guest --</option>
                    {guests.map((g) => (
                        <option key={g.id} value={g.id}>
                            {g.firstName} {g.lastName}
                        </option>
                    ))}
                </select>
                <button type="submit">Go</button>
            </form>
        </div>
    );
}
