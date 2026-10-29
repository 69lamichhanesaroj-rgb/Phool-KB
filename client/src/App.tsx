import {LoginPage} from "./LoginPage";
import {useState} from "react";
import { UsersPage } from "./UsersPage";
import type { User } from "./types";


export function App() {

    const [user, setUser] = useState<User | null>(null);

    if (user === null) {
        return <LoginPage onLogin={setUser}/>;

    }

    return <UsersPage me={user} onLogout={() => setUser(null)}/>;
}