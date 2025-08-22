import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./home.css"; // ⬅️ add this import

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
        navigate(`/explore?guestId=${selectedGuestId}`);
    };

    return (
        <div className="home">
            {/* Hero / image header */}
            <header className="hero">
                <button className="back-btn" aria-label="Back">←</button>
                <div className="brand">
                    <div className="anchor" aria-hidden="true">⚓</div>
                    <div className="brand-text">
                        <span className="brand-title">EXPLORE</span>
                        <span className="brand-sub">Polar Expeditions</span>
                    </div>
                </div>
            </header>

            {/* Form panel */}
            <main className="panel">
                <form className="form" onSubmit={handleSubmit}>
                    <label className="label" htmlFor="guest">Select a guest</label>
                    <select
                        id="guest"
                        className="input"
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

                    <button type="submit" className="btn-outline">
                        Go
                    </button>
                </form>

                <button
                    type="button"
                    className="btn-cta"
                    onClick={() => navigate("/")}
                >
                    Front Page
                </button>
            </main>
        </div>
    );
}
