// src/auth/index.js
import { events, router, routes } from '../main';
import { IUser, IPayload } from '../model';

// URL and endpoint constants
const API_URL = 'http://localhost:5000/api/'
const LOGIN_URL = API_URL + 'auth/token'
const SIGNUP_URL = API_URL + 'auth/signup'

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

    login(context, creds, routeName) {

        let onSuccess = (data: IVueHttpResponse) => {

            data.json<IPayload<IAccessToken>>().then((payload) => {

                localStorage.setItem(AuthenticationHelper.AccessTokenKey, payload.data.access_token);
                this.user.authenticated = true;
                events.$emit(events.global.login, Object.assign({}, this.user));

                if (routeName)
                    router.push({ name: routeName });
            });
        };

        let onError = err => context.error = err.body;

        context.$http.post(LOGIN_URL, creds, { emulateJSON: true })
            .then(onSuccess, onError);
    }

    signup(context, creds, routeName) {

        context.$http.post(SIGNUP_URL, creds, (data) => {

            localStorage.setItem(AuthenticationHelper.AccessTokenKey, data.access_token)
            this.user.authenticated = true;
            events.$emit(events.global.login, Object.assign({}, this.user));
            if (routeName)
                router.push({ name: routeName });

        }, err => context.error = err.body);
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