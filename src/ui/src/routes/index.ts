import VueRouter = require('vue-router');
import { RouteConfig as routeConfig } from './route-config';
export { RouteNames } from './route-names';
export { RouteGuards } from './route-guards';

export const routeOptions: VueRouter.RouterOptions = {
  routes: routeConfig,
  mode: 'hash',
  linkActiveClass: 'active'
};