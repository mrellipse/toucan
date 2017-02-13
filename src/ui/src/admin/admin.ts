import Vue = require('vue');
import Vuex = require('vuex');
import VueRouter = require('vue-router');
import VueI18n = require('vue-i18n');
import Vuelidate = require('vuelidate');
import { default as Axios } from 'axios';
import { RouteGuards, RouteNames, RouterOptions } from './routes';
import { AuthenticationHelper } from '../helpers';
import { RootStoreTypes } from './store';
import { Loader } from '../components';
import { Locales } from '../locales';
import { AreaNavigation } from './navigation/navigation';
import { AreaFooter } from './footer/footer';

Vue.use(Vuex);

import './admin.scss';
import { Store } from './store/store';

Vue.use(<any>VueI18n); // languages

Object.keys(Locales).forEach((lang) => {
  (<any>Vue).locale(lang, Locales[lang]);
})

Vue.use(Vuelidate.default); // validation

(<any>Vue).config.lang = 'en';

Vue.use(VueRouter); // router

const router = new VueRouter(RouterOptions);
router.beforeEach(RouteGuards(RouteNames.login));

export const app = new Vue({

  router,

  components: {
    AreaFooter,
    Loader,
    Navigation: AreaNavigation
  },

  store: Store,

  created() {
    let token = AuthenticationHelper.getAccessToken();
    
    Store.dispatch(RootStoreTypes.common.updateUser, token);
    Store.dispatch(RootStoreTypes.common.loadingState, false);
  },

  computed: {

    isLoading: () => Store.state.common.isLoading
  }

}).$mount('#app');
