import { useLocation, Link } from "react-router-dom";

export default function Explore() {
    const location = useLocation();
    const state = location.state as { name?: string } | null;

    return (
        <div style={{ padding: "2rem" }}>
            {state?.name ? (
                <h1>Hi, {state.name}!</h1>
            ) : (
                <>
                    <h1>No name provided</h1>
                    <Link to="/">Go back</Link>
                </>
            )}
        </div>
    );
}
