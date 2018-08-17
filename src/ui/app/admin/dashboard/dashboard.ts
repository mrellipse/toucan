import Vue from "vue";
import { Route, RouteConfig as VueRouteConfig } from "vue-router";
import Component from "vue-class-component";
import { SecurityRoleClaims } from "../../model";

@Component({
  template: require("./dashboard.html")
})
export class AreaDashboard extends Vue {}

export const DashboardRouteConfig: VueRouteConfig[] = [
  {
    component: AreaDashboard,
    name: "dashboard",
    path: "",
    meta: {
      claims: [SecurityRoleClaims.Admin]
    }
  },
  {
    component: AreaDashboard,
    name: "home",
    path: "home",
    meta: {
      claims: [SecurityRoleClaims.Admin]
    }
  }
];
