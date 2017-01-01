import Vue = require('vue');
import Component from 'vue-class-component';
import AuthenticationHelper from '../../helpers/authentication';
import { ICredential } from '../../model/credential';
import { auth, routes } from '../../main';

@Component({
    name: 'Signup',
    template: require('./signup.html')
})
export class Signup extends Vue {

    credentials: ICredential = { username: '', password: '' };
    error: string = "";

    submit(): void {

        var credentials = {
            username: this.credentials.username,
            password: this.credentials.password
        }

        auth.signup(this, Object.assign({}, this.credentials), routes.home)
    }
}

export default Signup;