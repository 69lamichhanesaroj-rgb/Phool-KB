// The three roles from the assignment
export type Role = "user" | "superuser" | "admin";

// What a user looks like. There is no password here on purpose,
// the frontend should never receive a password.
export interface User {
    id: number;
    username: string;
    role: Role;
}
