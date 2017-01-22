import { ISignupOptions } from '../../model';
import { required, minLength, email, sameAs } from 'vuelidate/lib/validators';
import { ValidationRuleset } from 'vuelidate';
import { complexity, registered } from '../../validation'

export type TSignup = { signup: ISignupOptions, validationGroup: string[] };

export const validations: ValidationRuleset<TSignup> = {
    signup: {
        confirmPassword: {
            required,
            sameAsPassword: sameAs('password')
        },
        displayName: {
            required,
            minLength: minLength(2)
        },
        password: {
            required,
            complexity
        },
        userName: {
            required,
            email,
            registered
        }
    }
};