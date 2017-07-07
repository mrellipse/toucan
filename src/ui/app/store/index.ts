import { Module } from 'vuex';
import { Mutations } from './mutations';
import { Actions } from './actions';
import { ICommonState } from './state';

export { ICommonState } from './state';
export { IStoreService, StoreService } from './store-service'
export { Store } from 'vuex';
export { StoreTypes } from './types';

export const CommonModule: Module<ICommonState, {}> = {
    state: {
        isLoading: false,
        user: {
            authenticated: false,
            email: null,
            name: null,
            username: null,
            roles: [],
            verified: false,
            exp: null
        },
        userOptions: {
            locale: null
        },
        statusBar: {
            messageTypeId: null,
            text: null,
            title: null,
            uri: null,
            timeout: null
        }
    },
    mutations: Mutations,
    actions: Actions
};
