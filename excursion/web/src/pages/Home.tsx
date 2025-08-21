import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Home.css"; // import your CSS file

export default function Home() {
    const [userName, setUserName] = useState("");
    const navigate = useNavigate();

    function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        if (!userName.trim()) return;
        navigate("/explore", { state: { name: userName } });
    }

    return (
        <div className="home">
            {/* Heading over background */}
            <h1 className="hero-title">Hello, what’s your name?</h1>

            {/* Form panel at the bottom */}
            <div className="form-panel">
                <form className="form" onSubmit={handleSubmit}>
                    <label htmlFor="fname">Your name</label>
                    <input
                        id="fname"
                        type="text"
                        className="text-input"
                        value={userName}
                        onChange={(e) => setUserName(e.target.value)}
                        placeholder="Enter your name"
                    />
                    <div className="spacer" />
                    <button type="submit" className="submit-btn">
                        Go
                    </button>
                </form>
            </div>
        </div>
    );
}
