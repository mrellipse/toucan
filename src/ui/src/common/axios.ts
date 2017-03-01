import { default as Axios } from 'axios';
import VueRouter = require('vue-router');
import { TokenHelper } from '../common';

let initialized: boolean = false;

export function UseAxios(router: VueRouter) {

    if (!initialized) {

        Axios.interceptors.request.use((config) => {

            if (!config.headers['Authorization']) {   // append authorization header
                let bearerToken = TokenHelper.getBearerToken();

                if (bearerToken.Authorization)
                    Object.assign(config.headers, bearerToken);
            }

            if (!config.maxRedirects || config.maxRedirects === 5)  // ensure axios does not follow redirects, so response interceptor below can push to app login page
                config.maxRedirects = 0;

            return config;
        });

        Axios.interceptors.response.use((config) => { 
            
            // if the server returns 200 OK + location header, this is the equivalent of a 302 redirection command 

            let location: string = config.headers['location'];

            if (config.status === 200 && location) {

                let redirectTo = '/' + location.split('/').slice(3).join('') + '/';

                router.replace(redirectTo);

            } else {
                return config;
            }
        });

        initialized = true;
    }
};