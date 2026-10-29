import type { User } from "./types.ts";

const users: User[] = [
    {id: 1, username: "admin", role: "admin"},
    {id: 2, username: "superuser", role: "superUser"},
    {id: 3, username: "user", role: "user"},
];

const passwords: Record<string, string> = {
    admin: "admin123",
    superuser: "superuser123",
    user: "user123",
};

const wait = () => new Promise((resolve) => setTimeout(resolve, 500));

export async function login(username: string, password: string): Promise<User> {
    await wait();
const found = users.find((u) => u.username === username);
    if(found && passwords [username] === password)
    {return found;}
    
    throw new Error("Invalid username or password");
    
    }
    
    export async function getUsers(): Promise<User[]> {
        await wait();
        return [...users];
    }
    
    export async function deleteUser(id: number): Promise<void> {
    await wait();
    const index = users.findIndex((u) => u.id === id);
    if (index === -1) throw new Error("User does not exist");
    users.splice(index, 1);

}