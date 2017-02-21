export interface IUser {
    authenticated: boolean;
    displayName?: string;
    email?: string;
    name?: string;
    roles: string[];
    username: string;
    verified?: boolean;
}

export const UserRoles = {
    Admin: 'Admin',
    User: 'User'
};