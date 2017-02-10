import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import jwtDecode = require('jwt-decode');

import { EventBus } from '../../events';
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

export class AuthenticationHelper implements IClaimsHelper {

    private static AccessTokenKey: string = 'id_token';
    public user: IUser = { authenticated: false, email: null, name: null, username: null, roles: [] };

    constructor() {

        var token = localStorage.getItem(AuthenticationHelper.AccessTokenKey);
        this.updateUser(token);

    }

    clearUserData(user: IUser): void {

        user.authenticated = false;
        user.email = null;
        user.name = null;
        user.username = null;
        user.roles = null;

    }

    isInRole(role: string): boolean {

        if (!this.user.roles)
            return false;
        else
            return this.user.roles.find(o => o.toLowerCase() === role.toLowerCase()) !== undefined;

    }

    login(credentials: ICredential) {

        let data = credentials;

        let defer = new Promise<boolean>(() => { });

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IAccessToken> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                let token = payload.data.access_token;

                localStorage.setItem(AuthenticationHelper.AccessTokenKey, token);
                this.updateUser(token);
                EventBus.$emit(EventBus.global.login, Object.assign({}, this.user));

                return true;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(LOGIN_URL, data)
            .then(onSuccess);
    }

    logout() {

        localStorage.removeItem(AuthenticationHelper.AccessTokenKey)
        this.updateUser(null);
        EventBus.$emit(EventBus.global.logout, Object.assign({}, this.user));

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

        let data = signup;

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IAccessToken> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                let token = payload.data.access_token;

                localStorage.setItem(AuthenticationHelper.AccessTokenKey, token);
                this.updateUser(token);
                EventBus.$emit(EventBus.global.login, Object.assign({}, this.user));

                return true;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(SIGNUP_URL, data)
            .then(onSuccess);

    }

    private updateUser(token: string): void {

        let user = this.user;

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

        } else {
            this.clearUserData(user);
        }

    }

    getAuthHeader() {

        return {
            'Authorization': 'Bearer ' + localStorage.getItem(AuthenticationHelper.AccessTokenKey)
        }

    }
}


export default AuthenticationHelper;
