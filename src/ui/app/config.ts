
const baseUri = 'https://localhost:5000';

export default {

    uri: {
        auth: baseUri + '/auth/',
        site: baseUri,
        services: baseUri + "/api/"
    },
    auth: {
        accessTokenKey: 'AUTH-LOCAL',
        externalProviderKey: 'AUTH-EXTERNAL'
    },
    uopt: 'UOPT',
    xsrf: {
        cookieName: 'XSRF-TOKEN',
        headerName: 'X-XSRF-TOKEN'
    }
};