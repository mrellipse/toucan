import Vue from "vue";
import Component from "vue-class-component";
import { Store } from "vuex";
import { State } from "vuex-class";
import {
  IRouteMixinData,
  IRouterMixinData
} from "../../../mixins/mixin-router";
import { ICommonState } from "../../../store";
import { ManageRoleService } from "../role-service";
import { ISearchResult, IRole, IUser } from "../../../model";
import { IAdminStoreState } from "../../store";
import { Toggle } from "../../../components";
import { IClaimsHelper, AuthenticationService } from "../../../services";

@Component({
  components: {
    toggle: Toggle
  },
  props: ["page", "pageSize"],
  template: require("./list.html")
})
export class RoleList extends Vue {
  claimsHelper: IClaimsHelper;
  currentPage: number = Object.assign((<any>this).page);
  currentPageSize: number = Object.assign((<any>this).pageSize);
  start: number = 1;
  end: number = 5;
  $route: IRouteMixinData;
  $router: IRouterMixinData;
  $store: Store<{ common: ICommonState }>;
  @State((state: IAdminStoreState) => state.common.user)
  user: IUser;

  private svc: ManageRoleService;
  private searchResults: ISearchResult<IRole> = null;

  public get total(): number {
    return this.searchResults ? this.searchResults.total : null;
  }

  public get roles(): IRole[] {
    return this.searchResults ? this.searchResults.items : [];
  }

  created() {
    this.claimsHelper = new AuthenticationService(this.$store);
    this.svc = new ManageRoleService(this.$store);
    this.search();
  }

  updateRoleStatus(role: IRole) {
    let data = Object.assign({}, role);

    this.svc.updateRoleStatus(data).catch((e: Error) => {
      this.$nextTick().then(() => (role.enabled = !role.enabled));
    });
  }

  public get onText() {
    return this.$t("dict.yes");
  }

  public get offText() {
    return this.$t("dict.no");
  }

  navigate(forward: boolean) {
    if (forward) this.currentPage++;
    else this.currentPage--;

    this.search();
  }

  private search() {
    let onSuccess = (value: ISearchResult<IRole>) => {
      if (value) {
        this.searchResults = value;

        let page = value.page;
        let pageSize = value.pageSize;
        let total = value.total;

        this.start = 1 + (page > 1 ? (page - 1) * pageSize : 0);
        this.end = page * pageSize >= total ? total : page * pageSize;
      }
    };

    this.svc.search(this.currentPage, this.currentPageSize).then(onSuccess);
  }
}
