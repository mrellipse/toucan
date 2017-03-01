
const baseUri = 'https://localhost:5000';

export const GlobalConfig = {

    uri: {
        auth: baseUri + '/auth/',
        site: baseUri,
        services: baseUri + "/api/"
    }
};

export default GlobalConfig;