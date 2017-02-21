import { Store, ActionContext, ActionTree } from 'vuex';
import { TokenHelper } from '../common';
import { IUser } from '../model';
import { ICommonState } from './state';
import { StoreTypes } from './types';

export const Actions: ActionTree<ICommonState, any> = {

    loading: (injectee: ActionContext<ICommonState, any>, loading: boolean) => {

        injectee.commit(StoreTypes.loadingState, loading);
    },

    updateUser: (injectee: ActionContext<ICommonState, any>, userData: string | IUser) => {

        let payload: IUser = null;

        if (typeof userData === 'string')
            payload = TokenHelper.parseUserToken(userData);
        else
            payload = userData

        injectee.commit(StoreTypes.updateUser, payload);
    }
};

export default Actions;