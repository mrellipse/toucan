import { Store, MutationTree } from 'vuex';
import { IUser } from '../model';
import { ICommonState } from './state';

export const Mutations: MutationTree<ICommonState> = {

    loading: (state: ICommonState, loading: boolean) => {
        state.isLoading = loading;
    },

    updateUser: (state: ICommonState, user: IUser) => {
        state.user = Object.assign({}, user);
    }

};

export default Mutations;