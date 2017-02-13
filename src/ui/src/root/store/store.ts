import { Store as VuexStore } from 'vuex';
import { Mutations } from './mutations';
import { Actions } from './actions';
import { IRootStoreState } from './state';
import { CommonModule } from '../../store';

export const Store = new VuexStore<IRootStoreState>({
    state: {},
    actions: Actions,
    mutations: Mutations,
    modules: {
        common: CommonModule
    }
});

export default Store;