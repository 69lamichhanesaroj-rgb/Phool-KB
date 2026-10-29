import {useState, type FormEvent} from "react";
import {login} from "./api.ts";
import type { User } from "./types";

type Props = {onLogin: (user: User) => void};

//LoginPage() is a component thats why its capital at first letter.so reacts know that its a component not an HTML tag. if it was loginpage() react would think its a function and not a component.
export function LoginPage({onLogin}: Props) {
const [username, setUsername] = useState("");
const [password, setPassword] = useState("");

const [error, setError] = useState<string | null>(null);
const [loading, setLoading] = useState(false);

    async function handleSubmit(e:FormEvent) {
        e.preventDefault();
        setError(null);
        setLoading(true);
        
        try{
            const user = await login({username, password});
            onLogin(user);
        } catch (err) {
            setError("Invalid username or password");
        } finally {
            setLoading(false);
        }
    }
    return (
        <main>
            <h1>Login</h1>
            <form onSubmit={handleSubmit}>
            < input
            value ={username}
            onChange ={(e) => setUsername(e.target.value)}
                
            />
                < input type="password"
                value={password}
                onChange={(e) => setPassword (e.target.value)
                }/>
                {error && <p style={{ color: "red" }}>{error}</p>}
                <button type="submit" disabled={loading}>{loading ? "Logging in..." : "Login"}</button>

                </form>
                <p> You typed: {username} </p>
        </main>
    );
}