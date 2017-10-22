import { Store, MutationTree } from 'vuex';
import { IStatusBarData, IUser } from '../model';
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

        user = user || {
            authenticated: false,
            cultureName: CookieHelper.getCultureName(),
            email: null,
            name: null,
            username: null,
            roles: [],
            verified: false,
            exp: null,
            timeZoneId: CookieHelper.getTimeZoneId()
        };
        
        state.user = Object.assign({}, user);
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