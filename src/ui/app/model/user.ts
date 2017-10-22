export interface IUser {
    authenticated: boolean;
    cultureName?: string;
    displayName?: string;
    email?: string;
    enabled?: boolean;
    name?: string;
    roles: string[];
    username: string;
    verified?: boolean;
    exp?: Date;
    userId?: number;
    timeZoneId?: string;
}

export const UserRoles = {
    Admin: 'Admin',
    User: 'User'
};