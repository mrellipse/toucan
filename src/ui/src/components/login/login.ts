import Vue = require('vue');
import { required, minLength, email } from 'vuelidate/lib/validators';
import Component from 'vue-class-component';
import { routes } from '../../main';
import { ICredential } from '../../model/credential';
import { Formatter } from 'vue-i18n';
import { AuthMixin, IAuthMixin, IAuthMixinData } from '../../mixins/mixin-auth';

@Component({
    name: 'Login',
    template: require('./login.html'),
    mixins: [AuthMixin],
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
export class Login extends Vue implements IAuthMixin {

    errorKey: string = "";
    password: string = 'password';

    get error(): string {
        return this.errorKey ? this.$t('validation.login.' + this.errorKey) : null;
    };

    submit(): void {

        var credentials: ICredential = {
            username: this.username,
            password: this.password
        }

        if(this.errorKey)
            this.errorKey = null;

        let onError = (error: { message: string }) => {
            this.errorKey = error.message;
        };

        this.$auth.login(credentials, routes.home)
            .catch(onError);
    }

    username: string = 'webmaster@toucan.org';

    $auth: IAuthMixinData
    $t: Formatter
}

export default Login;