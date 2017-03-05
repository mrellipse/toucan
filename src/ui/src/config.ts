
const baseUri = 'https://localhost:5000';

export const GlobalConfig = {

    uri: {
        auth: baseUri + '/auth/',
        site: baseUri,
        services: baseUri + "/api/"
    },
    auth: {
        accessTokenKey: 'AUTH-LOCAL',
        externalProviderKey: 'AUTH-EXTERNAL'
    },
    xsrf: {
        cookieName: 'XSRF-TOKEN',
        headerName: 'X-XSRF-TOKEN'
    }
};

export default GlobalConfig;