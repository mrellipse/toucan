import Vue = require('vue');
import { Store } from 'vuex';
import { RawLocation } from 'vue-router';
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import { required, minLength, email } from 'vuelidate/lib/validators';
import { PayloadMessageTypes, TokenHelper } from '../../common';
import { AuthenticationService, GoogleClient } from '../../services';
import { ICredential, ILoginProvider, ILoginClient, IPayload, IUser } from '../../model';
import { IRouterMixin, IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { StoreTypes } from '../../store';
import './login.scss';

@Component({
    name: 'Login',
    template: require('./login.html'),
    validations: {
        username: {
            email,
            required
        },
        password: {
            required,
            minLength: minLength(4)
        }
    }
})
export class Login extends Vue implements IRouterMixin {

    private auth: AuthenticationService = new AuthenticationService();
    private errorKey: string = "";
    private errorMessage: string = "";
    public externalProviders: ILoginClient[] = [new GoogleClient()];

    created() {

        let provider: ILoginProvider = TokenHelper.getProvider();

        if (provider) {
            Object.assign(this.provider, provider);
            this.resumeExternalLogin();
        }

        this.$watch('provider', this.onProviderChange, { deep: true });
    }

    get error(): string {

        if (this.errorKey) {

            let translationKey = 'validation.login.' + this.errorKey;
            let translation: string = this.$t(translationKey);

            if (translationKey !== translation)
                return translation;
            else
                return this.$t('dict.errors.unanticipated');
        }
    };

    login(): void {

        var credentials: ICredential = {
            username: this.username,
            password: this.password
        }

        let redirectTo: RawLocation = this.$route.query['redirect'] || { name: 'home' };

        if (this.errorKey) {
            this.errorKey = null;
            this.errorMessage = null;
        }

        let onError = (error: { message: string }) => {
            this.errorKey = error.message;
            this.errorMessage = error.message;
        };

        let onLogin = (value: IUser) => {
            this.$store.dispatch(StoreTypes.updateUser, value);
        }

        let onStoreDispatch = (o) => {
            this.$router.push(redirectTo);
        };

        this.auth.login(credentials)
            .then(onLogin)
            .then(onStoreDispatch)
            .catch(onError);
    }

    loginExternal(providerId: string): void {

        let client: ILoginClient = this.externalProviders.find((value) => value.providerId === providerId);

        if (!client) {

            throw new Error('Unknown provider');
        } else {

            let onSuccess = (nonce: string) => {

                this.provider.access_token = null;
                this.provider.responseUri = null;
                this.provider.providerId = providerId;
                this.provider.nonce = nonce;
                this.provider.uri = client.getUri(Object.assign({}, this.provider));
            };

            let nonce = this.auth.getAuthorizationNonce()
                .then(onSuccess);
        }
    }

    private resumeExternalLogin() {

        let provider: ILoginProvider = this.provider;

        let client: ILoginClient = this.externalProviders.find((value) => value.providerId === provider.providerId);

        let token = client ? client.getAccessToken(this.$route) : null;

        if (client && token) {

            provider.access_token = token;

            let redirectTo: RawLocation = this.$route.query['redirect'] || { name: 'home' };

            let onSuccess = (user: IUser) => {
                this.$store.dispatch(StoreTypes.updateUser, user);
                this.$router.push(redirectTo);
            };

            let onError = (error: { message: string }) => {
                this.errorKey = error.message;
                this.errorMessage = error.message;
            };

            this.auth.redeemAccessToken(provider)
                .then(onSuccess)
                .catch(onError);

        } else {
            throw new Error('Could not retreive access token for ' + provider.providerId);
        }
    }

    onProviderChange(val: ILoginProvider, oldVal: ILoginProvider) {

        if (val.nonce && val.providerId && val.uri && !val.access_token) {
            TokenHelper.setProvider(val);
            window.location.href = val.uri;
        }
    }

    provider: ILoginProvider = { nonce: null, providerId: null, uri: null, responseUri: null };

    password: string = 'password';

    username: string = 'webmaster@toucan.org';

    $route: IRouteMixinData;

    $router: IRouterMixinData;

    $store: Store<{}>;

    $t: Formatter
}

export default Login;