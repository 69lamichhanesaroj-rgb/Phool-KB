import { useEffect, useState } from "react";
import { deleteUser, getUsers } from "./api";
import { canCreate, canManage } from "./permissions";
import type { User } from "./types";
import { UserForm } from "./UserForm";

type Props = { me: User; onLogout: () => void };

export function UsersPage({ me, onLogout }: Props) {
    const [users, setUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // the form: is it open, and which user are we editing? (null = a new user)
    const [showForm, setShowForm] = useState(false);
    const [userToEdit, setUserToEdit] = useState<User | null>(null);

    async function loadUsers() {
        try {
            const list = await getUsers();
            setUsers(list);
        } catch (err) {
            setError("Could not load the users");
        } finally {
            setLoading(false);
        }
    }

    // runs one time when the page opens
    useEffect(() => {
        loadUsers();
    }, []);

    async function handleDelete(target: User) {
        if (!confirm(`Delete user "${target.username}"?`)) return;
        setError(null);
        try {
            await deleteUser(target.id);
            await loadUsers();
        } catch (err) {
            setError("Could not delete the user");
        }
    }

    function handleNew() {
        setUserToEdit(null);
        setShowForm(true);
    }

    function handleEdit(target: User) {
        setUserToEdit(target);
        setShowForm(true);
    }

    async function handleSaved() {
        setShowForm(false);
        await loadUsers();
    }

    return (
        <main className="page">
            <header>
                <h1>Users</h1>
                <div>
                    Logged in as {me.username} ({me.role})
                    <button onClick={onLogout}>Log out</button>
                </div>
            </header>

            {me.role === "user" && <p>You can only read users.</p>}
            {me.role === "superuser" && <p>You can create, edit and delete users.</p>}
            {me.role === "admin" && <p>You can create, edit and delete users, superusers and admins.</p>}

            {error && <p className="error">{error}</p>}

            {canCreate(me.role) && !showForm && (
                <button onClick={handleNew}>New user</button>
            )}

            {showForm && (
                <UserForm
                    key={userToEdit ? userToEdit.id : "new"}
                    me={me}
                    userToEdit={userToEdit}
                    onSaved={handleSaved}
                    onCancel={() => setShowForm(false)}
                />
            )}

            {loading ? (
                <p>Loading...</p>
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
                                    {canManage(me.role, u.role) && (
                                        <button onClick={() => handleEdit(u)}>Edit</button>
                                    )}
                                    {canManage(me.role, u.role) && u.id !== me.id && (
                                        <button onClick={() => handleDelete(u)}>Delete</button>
                                    )}
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
