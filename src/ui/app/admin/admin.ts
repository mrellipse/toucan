import Vue from 'vue';
import { default as Vuex } from 'vuex';
import { default as VueRouter } from 'vue-router';
import { default as Vuelidate } from 'vuelidate';
import { GlobalConfig, TokenHelper, UseAxios } from '../common';
import { Loader, StatusBar } from '../components';
import { VueSelect } from 'vue-select';
import { AreaFooter } from './footer/footer';
import { InitI18n } from '../locales';
import { IUser } from '../model';
import { AreaNavigation } from './navigation/navigation';
import * as Plugins from '../plugins';
import { RouteGuards, RouteNames, RouterOptions } from './routes';
import './admin.scss';
import { AdminStoreTypes } from './store';
import { Store } from './store/store';

Vue.use(Vuex);
Vue.use(Vuelidate);
Vue.use(VueRouter);

const router = new VueRouter(RouterOptions);

let options = {
  resolveUser: () => Store.state.common.user,
  forbiddenRouteName: RouteNames.forbidden,
  loginRouteName: RouteNames.login,
  verifyRouteName: RouteNames.verify,
  store: Store
};

router.beforeEach(RouteGuards(options));

Vue.use(Plugins.CommonsPlugin, {
  store: <never>Store,
});

let loadApp = (cb: (vue: Vue) => void) => {

  InitI18n().then((i18n) => {
    Vue.use(Plugins.UserOptionsPlugin, {
      i18n,
      store: <never>Store,
      watchLocaleChanges: true
    });

    UseAxios(router);

    Vue.component('status-bar', StatusBar);
    Vue.component('v-select', VueSelect);

    let app = new Vue({

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
                name: RouteNames.home,
                query: { hash: hash }
              });
            }
          }
        }

        let token = TokenHelper.getAccessToken();

        Store.dispatch(AdminStoreTypes.common.updateUser, token)
          .then(value => Store.dispatch(AdminStoreTypes.common.loadingState, false))
          .then(value => resumeExternalLogin());
      },

      computed: {

        isLoading: () => Store.state.common.isLoading

      }
    });

    return cb(app);
  });
}

(() => {
  window['bootstrap'].loadApp = loadApp;
})();
