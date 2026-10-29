import type { User } from "./types";
import { useEffect, useState } from "react";
import { canCreate, canManage } from "./permissions";
import { deleteUser, getUsers } from "./api";

type Props = { me : User; onLogout: () => void };


export function UsersPage({ me, onLogout }: Props) {
    const [users, setUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    
    useEffect(() => {
        getUsers().then((list) => {
            setUsers(list);
            setLoading(false);
        });
    }, []);
    
    async function handleDelete(target: User) {
        if (!confirm(`Delete user "${target.username}"?`)) return;
        try{
                await deleteUser(target.id);
                setUsers(await getUsers());
                
    } catch (e) {
        setError(e instanceof Error ? e.message : "Delete failed");
    }
    }
    return (
        <main>
            <h1>Users</h1>
            <p>Logged in as {me.username} ({me.role}) </p> {me.role === "admin" && <p> You can CRUD users,superusers and admins </p>}
                {me.role === "user" && <p> You can only read users </p>}
                {me.role === "superUser" && <p> You can CRUD users (not admins or other super users) </p>}
            <button onClick={onLogout}>Log out</button>
            {loading ? (
            <p>Loading…</p>
            ) : (
            <table>
                <thead>
                <tr>
                    <th>ID</th>
                    <th>Username</th>
                    <th>Role</th>
                    {canCreate(me.role) && <th>Actions</th>}
                </tr>
                </thead>
                <tbody>
                {users.map((u) => (
                    <tr key={u.id}>
                        <td>{u.id}</td>
                        <td>{u.username}</td>
                        <td>{u.role}</td>
                        {canCreate(me.role) && (
                            <td>
                                <button onClick={() => handleDelete(u)}>Delete</button>
                            </td>
                        )}
                    </tr>
                ))}
                </tbody>
            </table>
            )}
        </main>
                );
}