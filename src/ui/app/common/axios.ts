import { default as Axios, AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
import VueRouter = require('vue-router');
import { GlobalConfig, TokenHelper } from '../common';

let initialized: boolean = false;

export function UseAxios(router: VueRouter) {

    if (!initialized) {

        Axios.interceptors.request.use((config: AxiosRequestConfig) => {

            if (!config.headers['Authorization']) {   // append authorization header
                let bearerToken = TokenHelper.getBearerToken();

                if (bearerToken.Authorization)
                    Object.assign(config.headers, bearerToken);
            }

            if (!config.maxRedirects || config.maxRedirects === 5)  // ensure axios does not follow redirects, so custom response interceptor below can push to app login page
                config.maxRedirects = 0;

            return config;
        });

        Axios.interceptors.response.use(undefined, (config: AxiosError) => {

            let response: AxiosResponse = config.response;

            if (response.status === 401) {
                let location: string = response.headers['location'] || response.headers['Location'];

                if (location) {
                    let redirectTo = '/' + location;
                    window.setTimeout(() => router.replace(redirectTo), 200);
                }
            }

            return config;
        });

        initialized = true;
    }
};