import { Store, ActionContext, ActionTree } from 'vuex';
import { IRootStoreState } from './state';
import { RootStoreTypes } from './types';

export const Actions: ActionTree<IRootStoreState,{}> = {

    apiCallContent: (injectee: ActionContext<IRootStoreState, any>, content: string ) => {

        injectee.commit(RootStoreTypes.apiCallContent, content);

    }
};