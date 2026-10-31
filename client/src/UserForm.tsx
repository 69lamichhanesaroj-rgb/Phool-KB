import { useState, type FormEvent } from "react";
import { createUser, updateUser } from "./api";
import { rolesICanManage } from "./permissions";
import type { Role, User } from "./types";

// me           = the logged in user
// userToEdit   = the user we are editing, or null when we create a new one
type Props = {
    me: User;
    userToEdit: User | null;
    onSaved: () => void;
    onCancel: () => void;
};

export function UserForm({ me, userToEdit, onSaved, onCancel }: Props) {
    // only show the roles this person is allowed to give
    const roles = rolesICanManage(me.role);

    // when we edit, start with the values of that user
    const [username, setUsername] = useState(userToEdit ? userToEdit.username : "");
    const [password, setPassword] = useState("");
    const [role, setRole] = useState<Role>(userToEdit ? userToEdit.role : "user");
    const [error, setError] = useState<string | null>(null);
    const [saving, setSaving] = useState(false);

    async function handleSubmit(e: FormEvent) {
        e.preventDefault();
        setError(null);
        setSaving(true);

        try {
            if (userToEdit) {
                await updateUser(userToEdit.id, username, role, password);
            } else {
                await createUser(username, password, role);
            }
            onSaved();
        } catch (err) {
            if (err instanceof Error) {
                setError(err.message);
            } else {
                setError("Something went wrong");
            }
        } finally {
            setSaving(false);
        }
    }

    return (
        <form className="userform" onSubmit={handleSubmit}>
            <h2>{userToEdit ? "Edit " + userToEdit.username : "New user"}</h2>

            <label>
                Username
                <input
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />
            </label>

            <label>
                Password {userToEdit && "(leave empty to keep the old one)"}
                <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required={!userToEdit}
                    minLength={8}
                />
            </label>

            <label>
                Role
                <select
                    value={role}
                    onChange={(e) => setRole(e.target.value as Role)}
                >
                    {roles.map((r) => (
                        <option key={r} value={r}>{r}</option>
                    ))}
                </select>
            </label>

            {error && <p className="error">{error}</p>}

            <button type="submit" disabled={saving}>
                {saving ? "Saving..." : "Save"}
            </button>
            <button type="button" onClick={onCancel}>Cancel</button>
        </form>
    );
}
