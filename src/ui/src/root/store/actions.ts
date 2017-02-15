import { Store, ActionContext, ActionTree } from 'vuex';
import { IRootStoreState } from './state';
import { RootStoreTypes } from './types';
import {} from ''

export const Actions: ActionTree<IRootStoreState,{}> = {

    secureContent: (injectee: ActionContext<IRootStoreState, any>, content: string ) => {

        injectee.commit(RootStoreTypes.secureContent, content);
    }
};

export default Actions;