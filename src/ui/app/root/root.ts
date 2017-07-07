import * as Vue from 'vue';
import { default as Vuex } from 'vuex';
import { default as VueRouter } from 'vue-router';
import { default as Vuelidate } from 'vuelidate';
import VueI18n from 'vue-i18n';
import { GlobalConfig, TokenHelper, UseAxios } from '../common';
import { Loader, StatusBar } from '../components';
import { AreaFooter } from './footer/footer';
import { i18n } from '../locales';
import { IUser } from '../model';
import { AreaNavigation } from './navigation/navigation';
import * as Plugins from '../plugins';
import { RouteGuards, RouteNames, RouterOptions } from './routes';
import './root.scss';
import { RootStoreTypes } from './store';

Vue.use(Vuex);
import { Store } from './store/store';  // global state management
Vue.use(Vuelidate); // validation
Vue.use(VueRouter); // router

const router = new VueRouter(RouterOptions);
let options = {
  resolveUser: () => Store.state.common.user,
  forbiddenRouteName: RouteNames.forbidden,
  loginRouteName: RouteNames.login.home,
  verifyRouteName: RouteNames.login.verify,
  store: Store
};

router.beforeEach(RouteGuards(options));

Vue.use(Plugins.CommonsPlugin, {
  store: <never>Store,
});

Vue.use(Plugins.UserOptionsPlugin, {
  key: GlobalConfig.uopt,
  default: { locale: 'en' },
  store: <never>Store,
  watchLocaleChanges: true
});

UseAxios(router);

Vue.component('status-bar', StatusBar);

export const app = new Vue({

  components: {
    AreaFooter,
    Loader,
    Navigation: AreaNavigation
  },

  i18n,

  router,

  store: Store,

  created() {

    // check if location hash has state/nonce value ...
    let resumeExternalLogin = () => {

      if (location.hash) {

        let hash = location.hash.substring(1);

        if (hash.indexOf("error") != -1 || hash.indexOf("state") != -1 || hash.indexOf("token") != -1) {
          router.push({
            name: RouteNames.login.home,
            query: { hash: hash }
          });
        }
      }
    }

    let token = TokenHelper.getAccessToken();

    Store.dispatch(RootStoreTypes.common.updateUser, token)
      .then(value => Store.dispatch(RootStoreTypes.common.loadingState, false))
      .then(value => resumeExternalLogin());
  },

  computed: {

    isLoading: () => Store.state.common.isLoading
  }

}).$mount('#app');