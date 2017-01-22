import Vue = require('vue');
import VueRouter = require('vue-router');
import VueI18n = require('vue-i18n');
import Vuelidate = require('vuelidate');
import { default as Axios } from 'axios';

import { Loader, Navigation } from './components';
import { EventBus } from './events';
import { locales } from './locales';
import { AuthenticationHelper } from './helpers/authentication';
import { routeConfig, RouteNames, routeOptions } from './routes';
import { ComponentOptions } from 'vue';
import 'src/style.scss';

Vue.use(<any>VueI18n); // languages

Object.keys(locales).forEach((lang) => {
  (<any>Vue).locale(lang, locales[lang]);
})

Vue.use(Vuelidate.default); // validation

(<any>Vue).config.lang = 'fr';

Vue.use(VueRouter); // router

export const events = new EventBus(); // global event bus

export const router = new VueRouter(routeOptions);

export const routes = RouteNames;

export const auth = new AuthenticationHelper();

export const app = new Vue({

  router,

  components: {
    Loader,
    Navigation
  },

  mixins: [],

  created() {
    events.$on(events.global.loading, (isLoading) => {
      (<any>this).isLoading = isLoading;
    });
  },

  data() {
    return {
      isLoading: false
    };
  }
}).$mount('#app');
