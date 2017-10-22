import * as Vue from 'vue';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import Component from 'vue-class-component';
import { IVuelidate, ValidationRuleset, Vuelidate, validationMixin } from 'vuelidate';
import { ICommonOptions } from '../../plugins';
import { AuthenticationService } from '../../services';
import { ISignupOptions, IUser } from '../../model';
import { IRouterMixinData } from '../../mixins/mixin-router';
import { ICommonState, StoreTypes } from '../../store';
import { validations, TSignup } from './signup-validate';

@Component({
    mixins: [validationMixin],
    name: 'Signup',
    template: require('./signup.html'),
    validations: validations
})
export class Signup extends Vue {

    private auth: AuthenticationService;

    @State((state: {common:ICommonState}) => state.common.user) user: IUser;

    constructor() {
        super();
    }

    created() {
        this.auth = new AuthenticationService(this.$store);
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

            signup.cultureName = this.user.cultureName;
            signup.timeZoneId = this.user.timeZoneId;

            this.auth.signup(signup)
                .then(onSignup)
                .then(onStoreDispatch);
        }
    }

    signup: ISignupOptions = {
        confirmPassword: 'P@ssw0rd',
        displayName: null,
        userName: null,
        password: 'P@ssw0rd'
    };

    $router: IRouterMixinData;

    $store: Store<{}>;

    $v: Vuelidate<TSignup>
}