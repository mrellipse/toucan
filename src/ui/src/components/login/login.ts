import Vue = require('vue');
import { required, minLength, email } from 'vuelidate/lib/validators';
import Component from 'vue-class-component';
import { auth, events, routes } from '../../main';
import { ICredential } from '../../model/credential';

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

    error: string = "";

    submit(): void {

        var credentials: ICredential = {
            username: this.username,
            password: this.password
        }

        let onError = (error: any) => {
            this.error = error;
        };

        auth.login(credentials, routes.home)
            .catch(onError);
    }
    password: string = 'password';
    username: string = 'webmaster@toucan.org';
}

export default Login;