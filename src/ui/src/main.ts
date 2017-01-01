import Vue = require('vue');
import VueRouter = require('vue-router');
import VueResource = require('vue-resource');
import { Loader, Navigation } from './components';
import { AuthenticationHelper } from './helpers/authentication';
import { EventBus } from './events';
import { routeNames, routeConfig } from './routes';

Vue.use(VueRouter);
Vue.use(VueResource);

import 'src/style.scss';

let options: VueRouter.RouterOptions = {
  routes: routeConfig,
  mode: 'hash',
  linkActiveClass: 'active'
};

export const events = new EventBus();

export const router = new VueRouter(options);

export const routes = routeNames;

export const auth = new AuthenticationHelper();

export const app = new Vue({

  router,

  components: {
    Loader,
    Navigation
  },

  data() {
    return {
      isLoading: false
    };
  },

  created() {
    events.$on(events.global.loading, (isLoading) => {
      (<any>this).isLoading = isLoading;
    });
  }
}).$mount('#app');
