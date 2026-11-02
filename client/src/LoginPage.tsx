import { useState, type FormEvent } from "react";
import { login } from "./api";
import type { User } from "./types";

type Props = { onLogin: (user: User) => void };

export function LoginPage({ onLogin }: Props) {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    async function handleSubmit(e: FormEvent) {
        e.preventDefault();
        setError(null);
        setLoading(true);

        try {
            const user = await login(username, password);
            onLogin(user);
        } catch (err) {
            setError("Invalid username or password");
        } finally {
            setLoading(false);
        }
    }

    return (
        <main className="login">
            <h1>Kvist &amp; Bjælke</h1>
            <h2>Login</h2>

            <form onSubmit={handleSubmit}>
                <label>
                    Username
                    <input
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                    />
                </label>

                <label>
                    Password
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </label>

                {error && <p className="error">{error}</p>}

                <button type="submit" disabled={loading}>
                    {loading ? "Logging in..." : "Login"}
                </button>
            </form>

            {/* Only for testing, delete before handing in */}
            <p className="hint">
                Test logins: admin / admin123, superuser / superuser123, user / user123
            </p>
        </main>
    );
}
