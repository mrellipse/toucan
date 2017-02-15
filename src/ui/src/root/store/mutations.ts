import { Store, MutationTree } from 'vuex';
import { IRootStoreState } from './state';

export const Mutations: MutationTree<IRootStoreState> = {
    
    secureContent: (state: IRootStoreState, content: string) => {

        state.secureContent = content;
    },
};

export default Mutations;