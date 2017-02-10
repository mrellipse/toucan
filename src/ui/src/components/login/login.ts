import Vue = require('vue');
import { RawLocation } from 'vue-router';
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { required, minLength, email } from 'vuelidate/lib/validators';
import { AuthenticationHelper } from '../../helpers';
import { ICredential } from '../../model/credential';
import { IRouterMixin, IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';

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

    private auth: AuthenticationHelper;
    errorKey: string = "";
    password: string = 'password';

    created() {
        this.auth = new AuthenticationHelper();
    }

    get error(): string {
        return this.errorKey ? this.$t('validation.login.' + this.errorKey) : null;
    };

    submit(): void {

        var credentials: ICredential = {
            username: this.username,
            password: this.password
        }

        let redirectTo: RawLocation = this.$route.query['redirect'] || { name: 'home' };

        if (this.errorKey)
            this.errorKey = null;

        let onError = (error: { message: string }) => {
            this.errorKey = error.message;
        };

        let onSuccess = (value: boolean) => {
            this.$router.push(redirectTo);
        }

        this.auth.login(credentials)
            .then(onSuccess)
            .catch(onError);
    }

    username: string = 'webmaster@toucan.org';

    $route: IRouteMixinData;

    $router: IRouterMixinData;

    $t: Formatter
}

export default Login;