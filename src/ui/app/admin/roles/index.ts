import { Route, RouteConfig as VueRouteConfig } from "vue-router";
import { RoleList } from "./actions/list";
import { ReadRole } from "./actions/read";
import { CreateRole } from "./actions/create";
import { UpdateRole } from "./actions/update";

export const RoleRoutes: VueRouteConfig[] = [
  {
    component: RoleList,
    name: "listRoles",
    path: "roles",
    props: function(route: Route) {
      let page = Number.parseInt(route.query["page"]);
      let pageSize = Number.parseInt(route.query["pageSize"]);

      return {
        page: isNaN(page) ? 1 : page,
        pageSize: isNaN(pageSize) ? 5 : pageSize
      };
    }
  },
  {
    component: UpdateRole,
    name: "updateRole",
    path: "role/update/:id",
    props: true
  },
  {
    component: ReadRole,
    name: "viewRole",
    path: "role/:id",
    props: true
  },
  {
    component: CreateRole,
    name: "createRole",
    path: "role/create/:id",
    props: true
  }
];