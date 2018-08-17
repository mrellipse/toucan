import Vue from "vue";
import { Store } from "vuex";
import Component from "vue-class-component";
import { Vuelidate, validationMixin } from "vuelidate";
import { ManageRoleService, IGetRolePayload } from "../role-service";
import { IRouteMixinData } from "../../../mixins/mixin-router";
import { IRole, DefaultRole, ISecurityClaim } from "../../../model";
import { ICommonOptions, IVueSelectOption } from "../../../plugins";

const props = {
  id: { required: true }
};

@Component({
  mixins: [validationMixin],
  props: props,
  template: require("./read.html")
})
export class ReadRole extends Vue {
  private svc: ManageRoleService;
  $common: ICommonOptions;
  $route: IRouteMixinData;
  $store: Store<{}>;
  $v: Vuelidate<any>;
  availableClaims: ISecurityClaim[] = [];
  id: string = this.id;
  init: boolean = false;
  parentRoles: IVueSelectOption[];
  role: IRole = DefaultRole;
  selectedParentRole: IVueSelectOption = null;
  systemRoles: IRole[] = [];

  created() {
    let svc = (this.svc = new ManageRoleService(this.$store));

    if (this.id) {
      this.svc.getRole(this.id).then(this.onGetRole);
    }
  }

  cloneRole() {
    this.$router.push({ name: "createRole", params: { id: this.role.roleId } });
  }
  
  public get lastUpdateText() {
    if (!this.role) return "";

    let userName = null;

    if (this.role.lastUpdatedByUser)
      userName = this.role.lastUpdatedByUser.displayName;

    if (!userName && this.role.createdByUser)
      userName = this.role.createdByUser.displayName;

    let dt = this.role.lastUpdatedOn || this.role.createdOn || new Date();
    let date = new Date(dt).toDateString();
    let time = new Date(dt).toTimeString().split(' ')[0];

    return `${userName} @ ${date} ${time}`;
  }

  onGetRole(value: IGetRolePayload) {
    this.availableClaims = value.availableClaims;
    this.role = value.role;
    this.systemRoles = value.systemRoles;

    let apply = claim => {
      let match = this.availableClaims.find(
        o => o.securityClaimId === claim.securityClaimId
      );

      claim.securityClaim = match;
    };

    this.role.securityClaims.forEach(apply);

    this.parentRoles = value.systemRoles.map(o => {
      return { id: o.roleId, label: o.name };
    });

    this.selectedParentRole = this.role.isSystemRole
      ? null
      : this.parentRoles.find(o => o.id === this.role.parentRoleId);

    this.init = true;
  }

  public yesOrNo(value: boolean) {
    return value ? this.$t("dict.yes") : this.$t("dict.no");
  }

  public get title() {
    return `${this.$t("dict.role")} : ${this.role.name}`;
  }
}