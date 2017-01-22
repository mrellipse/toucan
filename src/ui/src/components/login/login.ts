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

    error: string = "";
    password: string = 'password';

    submit(): void {

        var credentials: ICredential = {
            username: this.username,
            password: this.password
        }

        let onError = (error: any) => {
            this.error = error;
        };

        this.$a.login(credentials, routes.home)
            .catch(onError);
    }

    username: string = 'webmaster@toucan.org';

    $a: IAuthMixinData
    $t: Formatter
}

export default Login;