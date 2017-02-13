import { Store as VuexStore } from 'vuex';
import { Mutations } from './mutations';
import { Actions } from './actions';
import { IAdminStoreState } from './state';
import { CommonModule } from '../../store';

export const Store = new VuexStore<IAdminStoreState>({
    state: {},
    actions: Actions,
    mutations: Mutations,
    modules: {
        common: CommonModule
    }
});

export default Store;