import { Route } from 'vue-router';

export const LoginProviders = {
    Local: 'LOCAL AUTHORITY',
    Google: 'GOOGLE',
    Microsoft: 'MICROSOFT'
};

export interface ILoginProvider {
    nonce: string;
    providerId: string;
    responseUri: string;
    uri: string;
    access_token?: string;
}

export interface ILoginClient {
    clientId: string;
    iconClass: string;
    localeKeys: { name: string };
    providerId: string;
    getAccessToken(route: Route): string;
    getUri(provider: ILoginProvider): string;
}