import { Store, MutationTree } from 'vuex';
import { IRootStoreState } from './state';

export const Mutations: MutationTree<IRootStoreState> = {
    
    apiCallContent: (state: IRootStoreState, content: string) => {

        state.apiCallContent = content;
    },
};