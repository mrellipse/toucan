import { RouterOptions as VueRouterOptions } from 'vue-router';
import { RouteConfig } from './route-config';

export { RouteGuards } from '../../routes/route-guards';
export { RouteNames } from './route-names';

export const RouterOptions: VueRouterOptions = {
  base: '/',
  mode: 'history',
  linkActiveClass: 'active',
  routes: RouteConfig,
};