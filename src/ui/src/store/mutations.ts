import { Store, MutationTree } from 'vuex';
import { IStatusBarData, IUser, IUserOptions } from '../model';
import { ICommonState } from './state';

export const Mutations: MutationTree<ICommonState> = {

    loading: (state: ICommonState, loading: boolean) => {
        state.isLoading = loading;
    },

    updateLocale: (state: ICommonState, lang: string) => {
        let user = Object.assign({}, state.userOptions);
        user.locale = lang;
        state.userOptions = user;
    },

    updateUser: (state: ICommonState, user: IUser) => {
        state.user = Object.assign({}, user);
    },

    updateUserOptions: (state: ICommonState, userOptions: IUserOptions) => {
        state.userOptions = Object.assign({}, userOptions);
    },

    updateStatusBar: (state: ICommonState, data: IStatusBarData) => {
        state.statusBar = Object.assign({}, data);
    }
};

export default Mutations;