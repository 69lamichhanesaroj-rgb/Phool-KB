import type { Role } from "./types";

const manageable: Record<Role,Role[]> = {
    user: [],
    superUser: ["user"],
    admin: ["user", "superUser", "admin"],
};
export const canCreate = (me: Role) => manageable[me].length > 0;
export const canManage = (me: Role, target: Role) => manageable[me].includes(target);
}