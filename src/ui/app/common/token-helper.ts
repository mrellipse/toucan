import { default as jwtDecode } from 'jwt-decode';
import { default as base64 } from 'base-64';
import { IJwtToken, ILoginProvider, IUser } from '../model';
import { default as GlobalConfig } from '../config';

export class TokenHelper {

    public static getAccessToken(): string {

        return localStorage.getItem(GlobalConfig.auth.accessTokenKey);
    }

    public static setAccessToken(token: string): void {

        return localStorage.setItem(GlobalConfig.auth.accessTokenKey, token);
    }

    public static removeAccessToken(): void {

        return localStorage.removeItem(GlobalConfig.auth.accessTokenKey);
    }

    public static getProvider(): ILoginProvider {

        let value = localStorage.getItem(GlobalConfig.auth.externalProviderKey);
        return value ? JSON.parse(base64.decode(value)) : null;
    }

    public static setProvider(provider: ILoginProvider): void {

        localStorage.setItem(GlobalConfig.auth.externalProviderKey, base64.encode(JSON.stringify(provider)));
    }

    public static removeProvider(): void {

        localStorage.removeItem(GlobalConfig.auth.externalProviderKey);
    }

    public static parseUserToken(token: string): IUser {

        let user: IUser = { authenticated: false, cultureName: null, displayName: null, email: null, name: null, username: null, roles: [], verified: false };

        if (token) {

            let decodedToken: IJwtToken = jwtDecode(token);

            if (!user.authenticated)
                user.authenticated = true;

            let name = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
            let roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
            let sid = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid'];
            let timeZoneId = decodedToken['TimeZoneId'];

            user.cultureName = decodedToken['CultureName'];
            user.displayName = name ? name[1] : null;
            user.email = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || null;
            user.name = user.email;
            user.roles = Array.isArray(roles) ? roles : [roles];
            user.verified = decodedToken['Verified'] === 'true' ? true : false;
            user.exp = new Date(decodedToken.exp * 1000);
            user.userId = sid;
            user.timeZoneId = timeZoneId;
        }

        return user;
    }

    public static getBearerToken() {

        let token = localStorage.getItem(GlobalConfig.auth.accessTokenKey);

        return {
            Authorization: token ? 'Bearer ' + localStorage.getItem(GlobalConfig.auth.accessTokenKey) : null
        };

    }

    public static isTokenCurrent(value: string | IUser) {

        let user: IUser = null;

        if (typeof value === 'string') {
            user = TokenHelper.parseUserToken(value);
        } else {
            user = value;
        }

        if (!user)
            return null;
        else
            return user.exp && user.exp > new Date();
    }
}
