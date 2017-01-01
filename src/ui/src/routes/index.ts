import VueRouter = require('vue-router');
import { Home, Login, PageNotFound, Signup } from '../components';

export const routeNames = {
  home: 'home',
  login: 'login',
  signup: 'signup'
}

export const routeConfig: VueRouter.RouteConfig[] = [
  {
    component: Home,
    name: routeNames.home,
    path: '/'
  },
  {
    component: Login,
    name: routeNames.login,
    path: '/login'
  },
  {
    component: Signup,
    name: routeNames.signup,
    path: '/signup',
  },
  {
    component: PageNotFound,
    path: '*'
  }
];