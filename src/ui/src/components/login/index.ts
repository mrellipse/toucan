import Vue = require('vue');
import Component from 'vue-class-component';
import { auth, events, routes } from '../../main';
import { ICredential } from '../../model/credential';

@Component({
    name: 'Login',
    template: require('./login.html')
})
export class Login extends Vue {

    credentials: ICredential = { username: 'webmaster@toucan.org', password: 'password' };
    error: string = "";

    submit(): void {

        var credentials = {
            username: this.credentials.username,
            password: this.credentials.password
        }

        let onError = (error: any) => {
            this.error = error;
        };

        auth.login(Object.assign({}, this.credentials), routes.home)
            .catch(onError);
    }
}

export default Login;