import jwtDecode = require('jwt-decode');
import { IJwtToken, ILoginProvider, IUser } from '../model';
import base64 = require('base-64');

const ACCESS_TOKEN_KEY: string = 'loacckey';
const PROVIDER_KEY = 'exprvkey';

export class TokenHelper {

    public static getAccessToken(): string {

        return localStorage.getItem(ACCESS_TOKEN_KEY);
    }

    public static setAccessToken(token: string): void {

        return localStorage.setItem(ACCESS_TOKEN_KEY, token);
    }

    public static removeAccessToken(): void {

        return localStorage.removeItem(ACCESS_TOKEN_KEY);
    }

    public static getProvider(): ILoginProvider {
        let value = localStorage.getItem(PROVIDER_KEY);
        return value ? JSON.parse(base64.decode(value)) : null;
    }

    public static setProvider(provider: ILoginProvider): void {

        localStorage.setItem(PROVIDER_KEY, base64.encode(JSON.stringify(provider)));
    }

    public static removeProvider(): void {

        localStorage.removeItem(PROVIDER_KEY);
    }

    public static parseUserToken(token: string): IUser {

        let user: IUser = { authenticated: false, email: null, name: null, username: null, roles: [], verified: false };

        if (token) {

            let decodedToken: IJwtToken = jwtDecode(token);

            if (!user.authenticated)
                user.authenticated = true;

            let name = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
            let roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

            user.displayName = name ? name[1] : null;
            user.email = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || null;
            user.name = user.email;
            user.roles = Array.isArray(roles) ? roles : [roles];
            user.verified = decodedToken['Verified'] === 'true' ? true : false;
            user.exp = new Date(decodedToken.exp * 1000);
        }

        return user;
    }

    public static getBearerToken() {

        let token = localStorage.getItem(ACCESS_TOKEN_KEY);

        return {
            Authorization: token ? 'Bearer ' + localStorage.getItem(ACCESS_TOKEN_KEY) : null
        };

    }
}

export default TokenHelper;
