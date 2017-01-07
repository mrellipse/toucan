import Vue = require('vue');
import Component from 'vue-class-component';
import AuthenticationHelper from '../../helpers/authentication';
import { IPayload, ISignupOptions } from '../../model';
import { auth, routes } from '../../main';
import { PayloadMessageTypes } from '../../helpers/message';
import { Patterns } from '../../helpers/pattern';

@Component({
    name: 'Signup',
    template: require('./signup.html')
})
export class Signup extends Vue {

    signup: ISignupOptions = {
        confirmPassword: '',
        confirmPasswordValidated: false,
        userName: '',
        userNameValidated: false,
        displayName: '',
        displayNameValidated: false,
        password: '',
        passwordValidated: false
    };
    error: string = "";

    submit(): void {

        if (!this.error)
            auth.signup(Object.assign({}, this.signup), routes.home);
            
    }

    private setError(msg: string): void {
        this.error = msg;
    }

    get formValidated() {
        return this.signup.confirmPasswordValidated && this.signup.displayNameValidated && this.signup.userNameValidated && this.signup.passwordValidated;
    }

    confirmPassword(): void {

        if (this.signup.password && this.signup.confirmPassword && this.signup.password !== this.signup.confirmPassword) {
            this.setError('Confirm password does not match');
            this.signup.confirmPasswordValidated = false;
        } else {
            this.setError('');
            this.signup.confirmPasswordValidated = true;
        }
    }

    validateDisplayName(): void {

        if (this.signup.displayName) {
            if (!Patterns.DisplayName.test(this.signup.displayName)) {
                this.setError('Please provide a valid display name');
                this.signup.displayNameValidated = false;
                return;
            } else {
                this.setError('');
                this.signup.displayNameValidated = true;
            }
        }
    }

    validatePassword(): void {

        if (this.signup.password) {
            if (!Patterns.Password.test(this.signup.password)) {
                this.setError('Please provide a valid password');
                this.signup.passwordValidated = false;
                return;
            } else {
                this.setError('');
                this.signup.passwordValidated = true;
            }
        }
    }

    validateUsername(cb: Function): void {

        if (this.signup.userName && !Patterns.Email.test(this.signup.userName)) {
            this.setError('Please provide a valid userName address');
            this.signup.userNameValidated = false;
            return;
        }

        let onSuccess = (payload: IPayload<boolean>) => {

            if (payload.message.messageTypeId === PayloadMessageTypes.failure) {
                this.signup.userNameValidated = false;
                this.setError(payload.message.text);
            }
            else {
                this.signup.userNameValidated = true;
                this.setError('');
            }
        }

        let payload = auth.validateUsername(this.signup.userName)
            .then(onSuccess)
            .catch(err => this.setError(err.body));
        ;
    }
}

export default Signup;