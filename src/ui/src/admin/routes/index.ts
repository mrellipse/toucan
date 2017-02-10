import { RouterOptions as VueRouterOptions } from 'vue-router';
import { RouteConfig } from './route-config';

export { RouteNames } from './route-names';
export { RouteGuards } from '../../routes/route-guards';

export const RouterOptions: VueRouterOptions = {
  routes: RouteConfig,
  mode: 'hash',
  linkActiveClass: 'active'
};
