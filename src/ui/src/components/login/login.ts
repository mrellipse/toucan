import Vue = require('vue');
import { RawLocation } from 'vue-router';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import Component from 'vue-class-component';
import { Formatter, KeypathChecker } from 'vue-i18n';
import { required, minLength, email } from 'vuelidate/lib/validators';
import { ICommonOptions } from '../../plugins';
import { PayloadMessageTypes, TokenHelper } from '../../common';
import { AuthenticationService, GoogleClient } from '../../services';
import { ICredential, ILoginProvider, ILoginClient, IPayload, IPayloadMessage, IUser } from '../../model';
import { IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { ICommonState, StoreTypes } from '../../store';
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
export class Login extends Vue {

    private auth: AuthenticationService = new AuthenticationService();
    public externalProviders: ILoginClient[] = [new GoogleClient()];

    @State((state: { common: ICommonState }) => state.common.user) user: IUser;

    created() {

        let provider: ILoginProvider = TokenHelper.getProvider();

        let onAuthError = (errorCode: string, errorDesc: string) => {

            let titleKey = 'login.' + errorCode;
            let textKey = 'login.' + errorDesc.replace('/', ' ');

            let msg: IPayloadMessage = {
                title: this.$te(titleKey) ? titleKey : errorCode,
                text: this.$te(textKey) ? textKey : errorDesc.replace('/', ' '),
                messageTypeId: PayloadMessageTypes.warning
            }

            this.$store.dispatch(StoreTypes.updateStatusBar, msg);

            if (errorCode === 'invalid_token') {
                console.info('invalid token. clearing vuex store and local storage.');
                this.$store.dispatch(StoreTypes.updateUser, null)
                    .then(value => TokenHelper.removeAccessToken());
            }
        }

        if (provider) {
            Object.assign(this.provider, provider);
            this.resumeExternalLogin();
        }

        this.$watch('provider', this.onProviderChange, { deep: true });

        let errorCode = this.$route.query['errorCode'];
        let hasValidToken = TokenHelper.isTokenCurrent(this.user);

        if (errorCode) {
            // probably a '302' redirection caused by global Axios interceptors (see src/common/axios.ts)
            onAuthError(errorCode, this.$route.query['errorDesc'].replace('/', ''));
        }
        else if (this.user.authenticated && !hasValidToken) {
            // the access token loaded from local storage has expired
            onAuthError('invalid_token', 'Token has expired');
        }
        else if (this.user.authenticated && hasValidToken && this.returnUrl) {
            // the access token is still valid, so redirect the user
            this.$router.replace(this.returnUrl);
        }
        else if (this.returnUrl) {

            let msg: IPayloadMessage = {
                text: 'login.secureRoute',
                title: null,
                messageTypeId: PayloadMessageTypes.warning
            }

            this.$store.dispatch(StoreTypes.updateStatusBar, msg);
        }
    }

    login(): void {

        var credentials: ICredential = {
            username: this.username,
            password: this.password
        }

        let returnUrl: RawLocation = this.returnUrl || { name: 'home' };

        this.$common.exec(this.auth.login(credentials))
            .then((value) => this.$store.dispatch(StoreTypes.updateUser, value))
            .then(() => this.$store.dispatch(StoreTypes.updateStatusBar, null))
            .then(() => this.$router.push(returnUrl))
    }

    loginExternal(providerId: string): void {

        let client: ILoginClient = this.externalProviders.find((value) => value.providerId === providerId);

        if (!client) {

            throw new Error('Unknown provider');
        } else {

            let onSuccess = (nonce: string) => {

                this.provider.access_token = null;
                this.provider.responseUri = null;
                this.provider.nonce = nonce;
                this.provider.providerId = providerId;
                this.provider.returnUri = this.returnUrl || null;
                this.provider.uri = client.getUri(Object.assign({}, this.provider));
            };

            this.$common.exec(this.auth.getAuthorizationNonce())
                .then(onSuccess);
        }
    }

    private resumeExternalLogin() {

        let provider: ILoginProvider = this.provider;

        let client: ILoginClient = this.externalProviders.find((value) => value.providerId === provider.providerId);

        let token = client ? client.getAccessToken(this.$route) : null;

        if (client && token) {

            provider.access_token = token;

            let returnUrl: RawLocation = provider.returnUri || { name: 'home' };

            let onSuccess = (user: IUser) => {
                this.$store.dispatch(StoreTypes.updateUser, user);
                this.$router.push(returnUrl);
            };

            this.$common.exec(this.auth.redeemAccessToken(provider))
                .then(onSuccess)
        }
    }

    onProviderChange(val: ILoginProvider, oldVal: ILoginProvider) {

        if (val.nonce && val.providerId && val.uri && !val.access_token) {
            TokenHelper.setProvider(val);
            window.location.href = val.uri;
        }
    }

    provider: ILoginProvider = { nonce: null, providerId: null, uri: null, responseUri: null, returnUri: null };

    password: string = 'P@ssword';

    public get returnUrl(): string {

        let returnUrl = this.$route.query['returnUrl'];

        return returnUrl ? returnUrl.replace(window.location.origin, '') : null;
    }

    username: string = 'webmaster@toucan.org';

    $common: ICommonOptions;

    $route: IRouteMixinData;

    $router: IRouterMixinData;

    $store: Store<{}>;

    $t: Formatter;

    $te: KeypathChecker;
}

export default Login;