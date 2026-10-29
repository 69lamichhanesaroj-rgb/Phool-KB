export type Role = "user" | "superUser" | "admin";

export interface User {
    id: number;
    username: string;
    role: Role;
}