import Vue = require('vue');
import VueRouter = require('vue-router');
import { Loader, Navigation } from './components';
import routes from './routes';

Vue.use(VueRouter);

import 'src/style.scss';

let options: VueRouter.RouterOptions = {
  routes: routes,
  mode: 'hash',
  linkActiveClass: 'active'
};

export const LoadingState = new Vue();

export const router = new VueRouter(options);

export const App = new Vue({
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
    LoadingState.$on('toggle', (isLoading) => {
      (<any>this).isLoading = isLoading;
    });
  }
}).$mount('#app');
