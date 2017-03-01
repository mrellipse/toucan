import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import jwtDecode = require('jwt-decode');
import base64 = require('base-64');
import { IAccessToken, ICredential, IJwtToken, ILoginProvider, IPayload, ISignupOptions, IUser } from '../../model';
import { GlobalConfig, PayloadMessageTypes, TokenHelper } from '../../common';
import { IClaimsHelper } from './claims-helper';

// URL and endpoint constants
const AUTH_URL = GlobalConfig.uri.auth;

const AUTH_ISSUE_NONCE = AUTH_URL + 'external/issuenonce';
const LOGIN_URL = AUTH_URL + 'token';
const SIGNUP_URL = AUTH_URL + 'signup';
const VALIDATE_USER_URL = AUTH_URL + 'validateuser';
const REDEEM_ACCESS_TOKEN = AUTH_URL + 'external/redeemtoken';
const VERIFICATION_CODE_URL = AUTH_URL + 'issueverificationcode';
const REDEEM_VERIFICATION_CODE = AUTH_URL + 'redeemverificationcode';

export { IClaimsHelper } from './claims-helper';

export class AuthenticationService implements IClaimsHelper {

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

    logout(): Promise<IUser> {

        let promise: Promise<IUser> = null;

        let user = { authenticated: false, email: null, name: null, username: null, roles: [], verified: false };
        try {
            TokenHelper.removeAccessToken();
            promise = Promise.resolve(user);
        } catch (e) {
            promise = Promise.reject(e);
        }

        return promise;
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

        let data = {
            accessToken: encodeURIComponent(provider.access_token),
            nonce: provider.nonce,
            providerId: provider.providerId
        }

        return Axios.post(REDEEM_ACCESS_TOKEN, data)
            .then(onSuccess);
    }

    redeemVerificationCode(code: string) {

        let onSuccess = (res) => {

            let payload: IPayload<IAccessToken> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {

                let token = payload.data.access_token;

                TokenHelper.setAccessToken(token);

                return TokenHelper.parseUserToken(token);

            } else {
                throw new Error(payload.message.text);
            }
        };

        let config = {
            params: { code: code }
        };

        return Axios.put(REDEEM_VERIFICATION_CODE, null, config)
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

    verify() {
        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<string> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                return payload.data;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.get(VERIFICATION_CODE_URL)
            .then(onSuccess);
    }
}

export default AuthenticationService;
