import * as Vue from 'vue';
import Component from 'vue-class-component';
import { RouteNames } from '../routes/route-names'
import { IRouterMixin, IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { ProfileSidebar } from '../profile/profile-sidebar';

@Component({
  components: {
    profile: ProfileSidebar
  },
  template: `<component v-bind:is="currentSidebar"></component>`
})
export class AreaSidebar extends Vue {

  get currentSidebar() {
    
    let routeName = this.$route.name;
    let sidebar: string = Object.keys(this.$options.components).find((value: string) => value === routeName);

    return sidebar;

  }

  $route: IRouteMixinData;

  $router: IRouterMixinData;
}