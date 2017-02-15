import Vue = require('vue');
import { Store } from 'vuex';
import Component from 'vue-class-component';
import { IVuelidate, ValidationRuleset, Vuelidate, validationMixin } from 'vuelidate';
import { AuthenticationHelper } from '../../helpers';
import { ISignupOptions, IUser } from '../../model';
import { IRouterMixinData } from '../../mixins/mixin-router';
import { StoreTypes } from '../../store';
import { validations, TSignup } from './signup-validate';

@Component({
    mixins: [validationMixin],
    name: 'Signup',
    template: require('./signup.html'),
    validations: validations
})
export class Signup extends Vue {

    private auth: AuthenticationHelper;

    constructor() {
        super();
    }

    created() {
        this.auth = new AuthenticationHelper;
    }

    get allowSubmit() {

        let error = this.$v.signup.$error || this.$v.signup.$invalid;
        return !error;
    }

    submit(): void {

        if (this.allowSubmit) {

            let signup = Object.assign({}, this.signup);

            let onSignup = (value: IUser) => {
                this.$store.dispatch(StoreTypes.updateUser, value);
            }

            let onStoreDispatch = (o) => {
                this.$router.push({ name: 'home' });
            };

            this.auth.signup(signup)
                .then(onSignup)
                .then(onStoreDispatch);
        }
    }

    signup: ISignupOptions = {
        confirmPassword: 'P@ssw0rd',
        displayName: 'iota',
        userName: 'iota@toucan.org',
        password: 'P@ssw0rd'
    };

    $router: IRouterMixinData;

    $store: Store<{}>;

    $v: Vuelidate<TSignup>
}

export default Signup;