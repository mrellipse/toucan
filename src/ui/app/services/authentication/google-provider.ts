import { default as ClientOAuth2 } from 'client-oauth2';
import { Route } from 'vue-router';
import { ILoginClient, ILoginProvider, LoginProviders } from '../../model';
import { GlobalConfig } from '../../common';

const GOOGLE_CLIENT_ID = '1012191853724-f9u0dcapj679ggokk5mblma4fbd77jmv.apps.googleusercontent.com';

const options: ClientOAuth2.Options = {
    clientId: GOOGLE_CLIENT_ID,
    accessTokenUri: 'https://accounts.google.com/o/oauth2/token',
    authorizationUri: 'https://accounts.google.com/o/oauth2/auth',
    redirectUri: GlobalConfig.uri.site,
    scopes: ['profile', 'email', 'openid'],
    query: {
        access_type: 'online',
        approval_prompt: 'auto',
        include_granted_scopes: 'false',
        response_type: 'token'
    }
};

export class GoogleClient implements ILoginClient {

    private client: ClientOAuth2;

    constructor() {

    }

    public get clientId(): string {
        return GOOGLE_CLIENT_ID;
    }

    public get iconClass(): string {
        return 'fa-google-plus';
    }

    public get localeKeys() {
        return {
            name: 'login.provider.google'
        };
    }

    public get providerId(): string {
        return LoginProviders.Google;
    }

    getAccessToken(route: Route): string {

        let hash = route.query['hash'];

        if (hash) {

            let dict = hash.split('&');

            let [key, value] = dict[0].split('=');

            if (key === 'error')
                throw new Error(value);

            if (key === 'state') {
                let [, accessToken] = dict[1].split('=');
                return accessToken;
            }
        }

        return null;
    }

    getUri(provider: ILoginProvider): string {

        if (!this.client)
            this.client = new ClientOAuth2(options);

        return this.client.token.getUri({ state: provider.nonce });
    }
}