import VueRouter = require('vue-router');
import { Home, Login, PageNotFound, Signup } from '../components';

export const RouteNames = {
  home: 'home',
  login: 'login',
  signup: 'signup'
}

export const routeConfig: VueRouter.RouteConfig[] = [
  {
    component: Home,
    name: RouteNames.home,
    path: '/'
  },
  {
    component: Login,
    name: RouteNames.login,
    path: '/login'
  },
  {
    component: Signup,
    name: RouteNames.signup,
    path: '/signup',
  },
  {
    component: PageNotFound,
    path: '*'
  }
];

export const routeOptions: VueRouter.RouterOptions = {
  routes: routeConfig,
  mode: 'hash',
  linkActiveClass: 'active'
};