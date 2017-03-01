import { Store, MutationTree } from 'vuex';
import { IStatusBarData, IUser } from '../model';
import { ICommonState } from './state';

export const Mutations: MutationTree<ICommonState> = {

    loading: (state: ICommonState, loading: boolean) => {
        state.isLoading = loading;
    },

    updateUser: (state: ICommonState, user: IUser) => {
        state.user = Object.assign({}, user);
    },

    updateStatusBar: (state: ICommonState, data: IStatusBarData) => {
        state.statusBar = Object.assign({}, data);
    }
};

export default Mutations;