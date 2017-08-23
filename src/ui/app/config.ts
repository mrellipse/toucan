
interface IUriConfig {
    auth?: string;
    site?: string;
    services?: string;
}

const uri : IUriConfig = {};

const addProp = (obj, propName, value) => {
    Object.defineProperty(obj, propName, {
        enumerable: false,
        get: () => {
            return window.location.protocol + '//' + window.location.host + value;
        }
    });
};

addProp(uri, 'auth', '/auth/');
addProp(uri, 'site', '');
addProp(uri, 'services', '/api/');

const config = {
    uri: uri,
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

export default config;
