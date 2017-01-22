import Vue = require('vue');
import Component from 'vue-class-component';
import { IVuelidate, ValidationRuleset, Vuelidate, validationMixin } from 'vuelidate';

import { IAuthMixinData , AuthMixin } from '../../mixins/mixin-auth';
import { ISignupOptions } from '../../model';
import { routes } from '../../main';
import { validations, TSignup } from './signup-validate';

@Component({
    mixins: [AuthMixin, validationMixin],
    name: 'Signup',
    template: require('./signup.html'),
    validations: validations
})
export class Signup extends Vue {

    constructor() {
        super();
    }

    get allowSubmit() {

        let error = this.$v.signup.$error || this.$v.signup.$invalid;
        return !error;
    }

    submit(): void {

        if (this.allowSubmit) {
            let signup = Object.assign({}, this.signup);
            this.$a.signup(signup, routes.home);
        }
    }

    signup: ISignupOptions = {
        confirmPassword: 'P@ssw0rd',
        displayName: 'iota',
        userName: 'iota@toucan.org',
        password: 'P@ssw0rd'
    };

    $a: IAuthMixinData;
    $v: Vuelidate<TSignup>
}

export default Signup;