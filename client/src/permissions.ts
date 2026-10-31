import type { Role } from "./types";

// Which roles is each role allowed to create, edit and delete?
const manageable: Record<Role, Role[]> = {
    user: [],                                // only reads
    superuser: ["user"],                     // can manage users
    admin: ["user", "superuser", "admin"],   // can manage everyone
};

// the roles this role is allowed to hand out
export function rolesICanManage(me: Role): Role[] {
    return manageable[me];
}

// can this role create users at all?
export function canCreate(me: Role): boolean {
    return manageable[me].length > 0;
}

// can this role edit or delete a user with the role "target"?
export function canManage(me: Role, target: Role): boolean {
    return manageable[me].includes(target);
}

// NOTE: this only decides what we SHOW on the screen.
// The backend must check the same rules, because anyone can skip the frontend.
