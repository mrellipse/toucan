export interface IUser {
    authenticated: boolean;
    displayName?: string;
    email?: string;
    enabled? : boolean;
    name?: string;
    roles: string[];
    username: string;
    verified?: boolean;
    exp?: Date;
}

export const UserRoles = {
    Admin: 'Admin',
    User: 'User'
};