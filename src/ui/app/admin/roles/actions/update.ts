import Vue from "vue";
import { Store } from "vuex";
import Component from "vue-class-component";
import { Vuelidate, validationMixin } from "vuelidate";
import {
  required,
  minLength,
  maxLength,
  alphaNum
} from "vuelidate/lib/validators";
import { ManageRoleService, IGetRolePayload } from "../role-service";
import { IRouteMixinData } from "../../../mixins/mixin-router";
import { PayloadMessageTypes } from "../../../common";
import {
  IStatusBarData,
  IRole,
  DefaultRole,
  ISecurityClaim
} from "../../../model";
import { StoreTypes } from "../../../store";
import { SecurityClaimsEditor } from "../claims-editor";
import { ICommonOptions, IVueSelectOption } from "../../../plugins";
import { Toggle } from "../../../components";
import { duplicate } from "../../../validation";

let existingRoleNames: string[] = [];

const components = {
  toggle: Toggle,
  securityClaimsEditor: SecurityClaimsEditor
};

const props = {
  id: { required: true }
};

const validations = {
  role: {
    name: {
      duplicate: duplicate(() => existingRoleNames, true),
      minLength: minLength(4),
      maxLength: maxLength(64),
      required
    },
    parentRoleId: function() {
      if (this.role.isSystemRole) return {};
      else return { required };
    }
  }
};

@Component({
  components: components,
  mixins: [validationMixin],
  props: props,
  template: require("./update.html"),
  validations: validations
})
export class UpdateRole extends Vue {
  private svc: ManageRoleService;
  $common: ICommonOptions;
  $route: IRouteMixinData;
  $store: Store<{}>;
  $v: Vuelidate<any>;
  availableClaims: ISecurityClaim[] = [];
  allowedValuesPattern: string;
  claimsValid: boolean = true;
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

  get allowSubmit() {
    return this.$v.role.$anyDirty && !this.$v.role.$invalid && this.claimsValid;
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

  submit() {
    let onSuccess = (value: IRole) => {
      if (value) {
        let msg: IStatusBarData = {
          text: "admin.role.updated",
          title: "dict.success",
          messageTypeId: PayloadMessageTypes.success
        };

        this.$store.dispatch(StoreTypes.updateStatusBar, msg);
      }
    };

    this.svc.updateRole(this.role).then(onSuccess);
  }

  onClaimsChange(value: any[]) {
    this.$v.role.$touch();
    this.claimsValid = value[0];
  }

  onGetRole(value: IGetRolePayload) {
    this.allowedValuesPattern = value.allowedValuesPattern;
    this.availableClaims = value.availableClaims;
    this.role = value.role;
    this.systemRoles = value.systemRoles;

    this.parentRoles = value.systemRoles.map(o => {
      return { id: o.roleId, label: o.name };
    });

    this.selectedParentRole = this.role.isSystemRole
      ? null
      : this.parentRoles.find(o => o.id === this.role.parentRoleId);

    existingRoleNames = value.existingRoles
      .filter(o => o.roleId !== value.role.roleId)
      .map(o => o.name);

    this.init = true;
  }

  public yesOrNo(value: boolean) {
    return value ? this.$t("dict.yes") : this.$t("dict.no");
  }

  public get onText() {
    return this.yesOrNo(true);
  }

  public get offText() {
    return this.yesOrNo(false);
  }

  public get title() {
    return `${this.$t("dict.role")} : ${this.role.name}`;
  }
}
