import * as Vue from 'vue';
import { default as Vuex } from 'vuex';
import { Mutations } from './mutations';
import { Actions } from './actions';
import { IRootStoreState } from './state';
import { CommonModule } from '../../store';

Vue.use(Vuex);

export const Store = new Vuex.Store<IRootStoreState>({
    state: {
        secureContent: null
    },
    actions: Actions,
    mutations: Mutations,
    modules: {
        common: CommonModule
    }
});

if ((<any>module).hot) {
  (<any>module).hot.accept([
    './actions',
    './mutations'
  ], () => {
    Store.hotUpdate({
      actions: require('./actions'),
      mutations: require('./mutations')
    })
  })
}

export default Store;