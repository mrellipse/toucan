// src/auth/index.js
import { events, router, routes } from '../main';
import { ICredential, IPayload, ISignupOptions, IUser } from '../model';
import { PayloadMessageTypes } from './message';
import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';

// URL and endpoint constants
const API_URL = 'http://localhost:5000/api/';
const LOGIN_URL = API_URL + 'auth/token';
const SIGNUP_URL = API_URL + 'auth/signup';
const VALIDATEUSERNAME_URL = API_URL + 'auth/validateusername';

interface IVueHttpResponse {
    url: string;
    body: Object | Blob | String;
    ok: boolean;
    status: number;
    statusText: string;
    json<T>(): Promise<T>;
    blob(): Blob;
    text(): string;
}

interface IAccessToken {
    access_token: string;
    expires_on: number;
}

export class AuthenticationHelper {

    private static AccessTokenKey: string = 'id_token';

    public user: IUser = { authenticated: false };

    constructor() {
        this.checkAuth();
    }

    login(credentials: ICredential, routeName: string) {

        let data = credentials;

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IAccessToken> = res.data;
            if (payload.message.messageTypeId === PayloadMessageTypes.success) {

                localStorage.setItem(AuthenticationHelper.AccessTokenKey, payload.data.access_token);
                this.user.authenticated = true;
                events.$emit(events.global.login, Object.assign({}, this.user));

                if (routeName)
                    router.push({ name: routeName });
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(LOGIN_URL, data)
            .then(onSuccess);
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

    signup(signup: ISignupOptions, routeName: string) {

        let data = signup;

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IAccessToken> = res.data;
            if (payload.message.messageTypeId === PayloadMessageTypes.success) {

                localStorage.setItem(AuthenticationHelper.AccessTokenKey, payload.data.access_token);
                this.user.authenticated = true;
                events.$emit(events.global.login, Object.assign({}, this.user));

                if (routeName)
                    router.push({ name: routeName });
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(SIGNUP_URL, data)
            .then(onSuccess);
    }

    logout() {

        localStorage.removeItem(AuthenticationHelper.AccessTokenKey)
        this.user.authenticated = false;
        events.$emit(events.global.logout, Object.assign({}, this.user));
        router.push({ name: routes.home });
    }

    checkAuth(): void {

        var jwt = localStorage.getItem(AuthenticationHelper.AccessTokenKey);

        this.user.authenticated = jwt ? true : false;

        events.$emit(events.global.login, Object.assign({}, this.user));
    }

    getAuthHeader() {
        return {
            'Authorization': 'Bearer ' + localStorage.getItem(AuthenticationHelper.AccessTokenKey)
        }
    }
}


export default AuthenticationHelper;