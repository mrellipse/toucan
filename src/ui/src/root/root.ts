import Vue = require('vue');
import { ComponentOptions } from 'vue';
import VueRouter = require('vue-router');
import VueI18n = require('vue-i18n');
import Vuelidate = require('vuelidate');
import { default as Axios } from 'axios';
import { Loader } from '../components';
import { EventBus } from '../events';
import { Locales } from '../locales';
import { AreaNavigation } from './navigation/navigation';
import { AreaFooter } from './footer/footer';
import { RouteGuards, RouteNames, RouterOptions } from './routes';

import './root.scss';

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

  mixins: [],

  created() {
    EventBus.$on(EventBus.global.loading, (isLoading) => {
      (<any>this).isLoading = isLoading;
    });
  },

  data() {
    return {
      isLoading: false
    };
  }
}).$mount('#app');
