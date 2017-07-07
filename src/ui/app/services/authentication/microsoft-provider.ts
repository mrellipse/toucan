import ClientOAuth2 = require('client-oauth2');
import { Route } from 'vue-router';
import { ILoginClient, ILoginProvider, LoginProviders } from '../../model';
import { GlobalConfig } from '../../common';

const MICROSOFT_CLIENT_ID = '56d7559c-2488-4ee2-9f4d-5bb2aa8b3b58';
// cf: https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-v2-protocols-implicit

const options: ClientOAuth2.Options = {
    clientId: MICROSOFT_CLIENT_ID,
    accessTokenUri: 'https://accounts.google.com/o/oauth2/token',
    authorizationUri: 'https://login.microsoftonline.com/common/oauth2/v2.0/authorize',
    redirectUri: GlobalConfig.uri.site,
    scopes: ['profile', 'email', 'openid'],
    query: {
        prompt: 'login',
        response_mode: 'fragment'
    }
};

export class MicrosoftClient implements ILoginClient {

    private client: ClientOAuth2;

    constructor() {

    }

    public get clientId(): string {
        return MICROSOFT_CLIENT_ID;
    }

    public get iconClass(): string {
        return 'fa-windows';
    }

    public get localeKeys() {
        return {
            name: 'login.provider.microsoft'
        };
    }

    public get providerId(): string {
        return LoginProviders.Microsoft;
    }

    getAccessToken(route: Route): string {

        let hash = route.query['hash'];
        let token: string = null;

        if (hash) {

            let dict = hash.split('&');

            let [key, value] = dict[0].split('=');

            if (key === 'error') {
                let [, desc] = dict[1].split('=');
                value = value.replace(/_/g, ' ');
                desc = decodeURIComponent(desc.replace(/\+/g, ' '));

                throw new Error(value.substring(0, 1).toUpperCase() + value.substring(1) + '. ' + desc);
            } else if (key === "id_token") {
                token = value;
            }
        }

        return token;
    }

    getUri(provider: ILoginProvider): string {

        if (!this.client) {
            let opts = Object.assign({}, options);
            opts.query['nonce'] = provider.nonce;
            this.client = new ClientOAuth2(opts);
        }

        let uri = this.client.token.getUri();
        uri = uri.replace('response_type=token', 'response_type=id_token');
        uri = uri.replace('state=', 'state=12345');

        return uri;
    }
}