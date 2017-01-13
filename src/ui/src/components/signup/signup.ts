import Vue = require('vue');
import Component from 'vue-class-component';
import { IVuelidate, ValidationRuleset, Vuelidate } from 'vuelidate';

import { ISignupOptions } from '../../model';
import { auth, routes } from '../../main';
import { validations, TSignup } from './signup-validate';

@Component({
    name: 'Signup',
    template: require('./signup.html'),
    validations: validations
})
export class Signup extends Vue implements IVuelidate<TSignup>{

    signup: ISignupOptions = {
        confirmPassword: '',
        displayName: '',
        userName: '',
        password: ''
    };

    get allowSubmit() {

        let error = this.$v.signup.$error || this.$v.signup.$invalid;
        return !error;
    }

    submit(): void {

        if (this.allowSubmit) {
            let signup = Object.assign({}, this.signup);
            auth.signup(signup, routes.home);
        }
    }

    $v: Vuelidate<TSignup>;
}

export default Signup;