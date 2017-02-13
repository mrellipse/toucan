import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import jwtDecode = require('jwt-decode');
import { ICredential, IJwtToken, IPayload, ISignupOptions, IUser } from '../../model';
import { PayloadMessageTypes } from './../message';
import { IClaimsHelper } from './claims-helper';
import { IAccessToken } from './token';

// URL and endpoint constants
const API_URL = 'http://localhost:5000/api/';
const LOGIN_URL = API_URL + 'auth/token';
const SIGNUP_URL = API_URL + 'auth/signup';
const VALIDATEUSERNAME_URL = API_URL + 'auth/validateusername';

export { IClaimsHelper } from './claims-helper';

export function parseUserToken(token: string): IUser {

    let user: IUser = { authenticated: false, email: null, name: null, username: null, roles: [] };

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
    }

    return user;
}

export class AuthenticationHelper implements IClaimsHelper {

    public static AccessTokenKey: string = 'id_token';

    public static getAccessToken(): string {

        return localStorage.getItem(AuthenticationHelper.AccessTokenKey);
    }

    public static getUserFromAccessToken(token?: string): IUser {

        return parseUserToken(token || AuthenticationHelper.getAccessToken());
    }

    constructor() { }

    isInRole(user: IUser, role: string): boolean {

        if (!user.roles)
            return false;
        else
            return user.roles.find(o => o.toLowerCase() === role.toLowerCase()) !== undefined;

    }

    login(credentials: ICredential) {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IAccessToken> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {

                let token = payload.data.access_token;
                let user = parseUserToken(token);

                localStorage.setItem(AuthenticationHelper.AccessTokenKey, token);

                return user;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(LOGIN_URL, credentials)
            .then(onSuccess);
    }

    logout(): IUser {

        localStorage.removeItem(AuthenticationHelper.AccessTokenKey)

        return { authenticated: false, email: null, name: null, username: null, roles: [] };
    }

    validateUsername(userName: string) {

        let config = {
            params: { userName: userName }
        };

        let onSuccess = (res: AxiosResponse) => {
            let payload: IPayload<boolean> = res.data;
            return payload;
        }

        return Axios.get(VALIDATEUSERNAME_URL, config)
            .then(onSuccess);
    }

    signup(signup: ISignupOptions) {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IAccessToken> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {

                let token = payload.data.access_token;
                let user = parseUserToken(token);
                
                localStorage.setItem(AuthenticationHelper.AccessTokenKey, token);

                return user;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(SIGNUP_URL, signup)
            .then(onSuccess);
    }

    getAuthHeader() {

        return {

            'Authorization': 'Bearer ' + localStorage.getItem(AuthenticationHelper.AccessTokenKey)
        }

    }
}

export default AuthenticationHelper;
