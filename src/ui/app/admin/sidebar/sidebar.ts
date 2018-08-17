import Vue from 'vue';
import Component from 'vue-class-component';
import { IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { SiteSettingsSidebar } from '../settings/settings-sidebar';
import { SiteReportsSidebar } from '../reports/reports-sidebar';

@Component({
  components: {
    siteReports: SiteReportsSidebar,
    siteSettings: SiteSettingsSidebar
  },
  template: `<component v-bind:is="currentSidebar"></component>`
})
export class AreaSidebar extends Vue {

  get currentSidebar(){

    let routeName = this.$route.name;
    let sidebar: string = Object.keys(this.$options.components).find((value: string) => value === routeName);

    return sidebar;
  }

  $route: IRouteMixinData;

  $router: IRouterMixinData;
}