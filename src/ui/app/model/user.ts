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

export interface IUserOptions {
    locale: string;
}

export const UserRoles = {
    Admin: 'Admin',
    User: 'User'
};