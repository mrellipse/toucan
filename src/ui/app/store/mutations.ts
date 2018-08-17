import { Store, MutationTree } from 'vuex';
import { IStatusBarData, IUser, DefaultUser } from '../model';
import { ICommonState } from './state';
import { CookieHelper } from '../common';

export const Mutations: MutationTree<ICommonState> = {

    loading: (state: ICommonState, loading: boolean) => {
        state.isLoading = loading;
    },

    updateLocale: (state: ICommonState, cultureName: string) => {
        let user = Object.assign({}, state.user);
        user.cultureName = cultureName;
        state.user = user;
    },

    updateUser: (state: ICommonState, user: IUser) => {
        state.user = Object.assign({}, user || DefaultUser);
    },

    updateStatusBar: (state: ICommonState, data: IStatusBarData) => {
        state.statusBar = Object.assign({}, data);
    },

    updateTimeZone: (state: ICommonState, timeZoneId: string) => {
        let user = Object.assign({}, state.user);
        user.timeZoneId = timeZoneId;
        state.user = user;
    },
};