import type { Role, User } from "./types";

const API_BASE = "http://localhost:8081";
const TOKEN_KEY = "phool.token";
let token: string | null = localStorage.getItem(TOKEN_KEY);

function setToken(value: string | null) {
    token = value;
    if (value === null) localStorage.removeItem(TOKEN_KEY);
    else localStorage.setItem(TOKEN_KEY, value);
}

type RoleRow = { id: number; roleName: Role };
let rolesCache: RoleRow[] | null = null;

async function getRoles(): Promise<RoleRow[]> {
    rolesCache ??= await request<RoleRow[]>("/api/role");
    return rolesCache;
}

async function roleIdOf(role: Role): Promise<number> {
    const roles = await getRoles();
    const found = roles.find((r) => r.roleName === role);
    if (!found) throw new Error(`Unknown role "${role}"`);
    return found.id;
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
    const headers = new Headers(init.headers);
    headers.set("Content-Type", "application/json");
    if (token) headers.set("Authorization", `Bearer ${token}`);

    let response: Response;
    try {
        response = await fetch(`${API_BASE}${path}`, { ...init, headers });
    } catch {
        throw new Error("Cannot reach the server. Is the API running?");
    }

    if (response.status === 401 && path !== "/api/auth/login") {
        setToken(null);
        throw new SessionExpiredError();
    }

    if (!response.ok) {
        let message = `Request failed (${response.status})`;
        try {
            const body = await response.json();
            if (body?.message) message = body.message;
        } catch {
            // body was empty or not json — keep the fallback
        }
        throw new Error(message);
    }

    if (response.status === 204) return undefined as T; // our endpoints return No Content
    return (await response.json()) as T;
}

type LoginResponse = { token: string; id: number; username: string; role: Role };

export async function login(username: string, password: string): Promise<User> {
    const result = await request<LoginResponse>("/api/auth/login", {
        method: "POST",
        body: JSON.stringify({ username, password }),
    });
    setToken(result.token);
    return { id: result.id, username: result.username, role: result.role };
}

export function logout() {
    setToken(null);
    rolesCache = null;
}

export async function getUsers(): Promise<User[]> {
    return request<User[]>("/api/user");
}

export async function createUser(username: string, password: string, role: Role): Promise<void> {
    await request<void>("/api/user", {
        method: "POST",
        body: JSON.stringify({ username, password, roleId: await roleIdOf(role) }),
    });
}

export async function updateUser(id: number, username: string, role: Role, password: string): Promise<void> {
    await request<void>(`/api/user/${id}`, {
        method: "PUT",
        body: JSON.stringify({ username, password, roleId: await roleIdOf(role) }),
    });
}

export async function deleteUser(id: number): Promise<void> {
    await request<void>(`/api/user/${id}`, { method: "DELETE" });
}

export class SessionExpiredError extends Error {
    constructor() {
        super("Your session expired. Please log in again.");
        this.name = "SessionExpiredError";
    }
}