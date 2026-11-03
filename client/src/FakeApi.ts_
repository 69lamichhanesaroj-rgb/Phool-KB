import type { Role, User } from "./types";

// ---------------------------------------------------------------
// FAKE BACKEND
// Everything here pretends to be the server. Later, each function
// will be replaced with a fetch() call to the ASP.NET API.
// The pages only use the function names, so they will not change.
// ---------------------------------------------------------------

const users: User[] = [
    { id: 1, username: "admin", role: "admin" },
    { id: 2, username: "superuser", role: "superuser" },
    { id: 3, username: "user", role: "user" },
];

// the fake passwords, saved by user id. A real server only stores hashes!
const passwords: Record<number, string> = {
    1: "admin123",
    2: "superuser123",
    3: "user123",
};

let nextId = 4;

// pretend the network is slow
const wait = () => new Promise((resolve) => setTimeout(resolve, 500));

export async function login(username: string, password: string): Promise<User> {
    await wait();
    const found = users.find((u) => u.username === username);
    if (found && passwords[found.id] === password) {
        return found;
    }
    throw new Error("Invalid username or password");
}

export async function getUsers(): Promise<User[]> {
    await wait();
    return [...users];
}

export async function createUser(username: string, password: string, role: Role): Promise<void> {
    await wait();
    const exists = users.find((u) => u.username === username);
    if (exists) {
        throw new Error("That username is already taken");
    }
    const newUser: User = { id: nextId, username: username, role: role };
    nextId = nextId + 1;
    users.push(newUser);
    passwords[newUser.id] = password;
}

// password can be "" which means: keep the old password
export async function updateUser(id: number, username: string, role: Role, password: string): Promise<void> {
    await wait();
    const user = users.find((u) => u.id === id);
    if (!user) {
        throw new Error("User does not exist");
    }
    user.username = username;
    user.role = role;
    if (password !== "") {
        passwords[id] = password;
    }
}

export async function deleteUser(id: number): Promise<void> {
    await wait();
    const index = users.findIndex((u) => u.id === id);
    if (index === -1) {
        throw new Error("User does not exist");
    }
    users.splice(index, 1);
}
