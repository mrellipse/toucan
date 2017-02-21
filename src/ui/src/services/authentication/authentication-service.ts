import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import jwtDecode = require('jwt-decode');
import base64 = require('base-64');
import { IAccessToken, ICredential, IJwtToken, ILoginProvider, IPayload, ISignupOptions, IUser } from '../../model';
import { PayloadMessageTypes, TokenHelper } from '../../common';
import { IClaimsHelper } from './claims-helper';

// URL and endpoint constants
const API_URL = 'https://localhost:5000/auth/';
const AUTH_ISSUE_NONCE = API_URL + 'external/issuenonce';
const LOGIN_URL = API_URL + 'token';
const SIGNUP_URL = API_URL + 'signup';
const VALIDATE_USER_URL = API_URL + 'validateuser';
const REDEEM_ACCESS_TOKEN = API_URL + 'external/redeemtoken';

export { IClaimsHelper } from './claims-helper';

export class AuthenticationService implements IClaimsHelper {

    public static getUser(): IUser {

        return TokenHelper.parseUserToken(TokenHelper.getAccessToken());
    }

    /**
     * Retreives a nonce value, so that incoming redirects from oauth providers can be validated by the server when redeeming access tokens ...
     */
    getAuthorizationNonce() {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<string> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                return payload.data;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(AUTH_ISSUE_NONCE)
            .then(onSuccess);
    }

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

                TokenHelper.setAccessToken(token);

                return TokenHelper.parseUserToken(token);
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(LOGIN_URL, credentials)
            .then(onSuccess);
    }

    logout(): IUser {

        TokenHelper.removeAccessToken();

        return { authenticated: false, email: null, name: null, username: null, roles: [] };
    }

    validateUser(userName: string) {

        let config = {
            params: { userName: userName }
        };

        let onSuccess = (res: AxiosResponse) => {
            let payload: IPayload<boolean> = res.data;
            return payload;
        }

        return Axios.get(VALIDATE_USER_URL, config)
            .then(onSuccess);
    }

    redeemAccessToken(provider: ILoginProvider) {

        let data = {
            accessToken: encodeURIComponent(provider.access_token),
            nonce: provider.nonce,
            providerId: provider.providerId
        }

        let onSuccess = (res) => {

            let payload: IPayload<IAccessToken> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {

                let token = payload.data.access_token;

                TokenHelper.setAccessToken(token);
                TokenHelper.removeProvider();

                return TokenHelper.parseUserToken(token);
            } else {
                throw new Error(payload.message.text);
            }
        };

        return Axios.post(REDEEM_ACCESS_TOKEN, data)
            .then(onSuccess);
    }

    signup(signup: ISignupOptions) {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IAccessToken> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {

                let token = payload.data.access_token;

                TokenHelper.setAccessToken(token);

                return TokenHelper.parseUserToken(token);
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.post(SIGNUP_URL, signup)
            .then(onSuccess);
    }
}

export default AuthenticationService;
