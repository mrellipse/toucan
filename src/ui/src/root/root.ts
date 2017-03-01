import Vue = require('vue');
import Vuex = require('vuex');
import VueRouter = require('vue-router');
import VueI18n = require('vue-i18n');
import Vuelidate = require('vuelidate');
import { RouteGuards, RouteNames, RouterOptions } from './routes';
import { TokenHelper, UseAxios } from '../common';
import { IUser } from '../model';
import { RootStoreTypes } from './store';
import { Loader, StatusBar } from '../components';
import { Locales } from '../locales';
import { AreaNavigation } from './navigation/navigation';
import { AreaFooter } from './footer/footer';

Vue.use(Vuex);

import { Store } from './store/store';
import './root.scss';

Vue.use(<any>VueI18n); // languages

Object.keys(Locales).forEach((lang) => {
  (<any>Vue).locale(lang, Locales[lang]);
})

Vue.use(Vuelidate.default); // validation

(<any>Vue).config.lang = 'en';

Vue.use(VueRouter); // router

const router = new VueRouter(RouterOptions);
let options = {
  resolveUser: () => Store.state.common.user,
  forbiddenRouteName: RouteNames.forbidden,
  loginRouteName: RouteNames.login.home,
  verifyRouteName: RouteNames.login.verify
};
router.beforeEach(RouteGuards(options));

UseAxios(router);

Vue.component('status-bar', StatusBar);

export const app = new Vue({

  components: {
    AreaFooter,
    Loader,
    Navigation: AreaNavigation
  },

  router,

  store: Store,

  created() {

    let token = TokenHelper.getAccessToken();

    Store.dispatch(RootStoreTypes.common.updateUser, token);
    Store.dispatch(RootStoreTypes.common.loadingState, false);

    // check if location hash has state/nonce value ...
    if (location.hash && location.hash.indexOf("state") != -1) {

      let hash = location.hash.substring(1);

      if (hash.indexOf("access_token") != -1 || hash.indexOf("error") != -1) {
        router.push({
          name: RouteNames.login.home,
          query: { hash: hash }
        });
      }
    }
  },

  computed: {

    isLoading: () => Store.state.common.isLoading
  }

}).$mount('#app');