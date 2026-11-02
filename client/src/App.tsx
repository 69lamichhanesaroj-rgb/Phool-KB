import { useState } from "react";
import "./index.css";
import { LoginPage } from "./LoginPage";
import { UsersPage } from "./UsersPage";
import type { User } from "./types";

export function App() {
    // who is logged in? null means nobody
    const [user, setUser] = useState<User | null>(null);

    // nobody logged in -> show the login page
    if (user === null) {
        return <LoginPage onLogin={setUser} />;
    }

    // somebody is logged in -> show the users page
    return <UsersPage me={user} onLogout={() => setUser(null)} />;
}

export default App;
