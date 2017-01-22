import Vue = require('vue');
import { AuthenticationHelper } from '../helpers/authentication';
import { auth } from '../main';

export { AuthenticationHelper } from '../helpers/authentication';

export const AuthMixin = {
    computed: {
        $a: () => auth
    }
}

export type IAuthMixinData = AuthenticationHelper;

export interface IAuthMixin {
    $a: IAuthMixinData;
}

export default AuthMixin;

