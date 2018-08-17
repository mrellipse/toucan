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
import { SecurityClaimsEditor } from "../claims-editor";
import { ICommonOptions, IVueSelectOption } from "../../../plugins";
import { StoreTypes } from "../../../store";
import { Toggle } from "../../../components";
import { duplicate } from "../../../validation";

let existingRoleIds: string[] = [];
let existingRoleNames: string[] = [];

const components = {
  toggle: Toggle,
  securityClaimsEditor: SecurityClaimsEditor
};

const props = {
  id: { required: true },
  clone: { required: false, default: false }
};

const validations = {
  role: {
    name: {
      duplicate: duplicate(() => existingRoleNames, true),
      minLength: minLength(4),
      maxLength: maxLength(64),
      required
    },
    parentRoleId: {
      required
    },
    roleId: {
      alphaNum,
      duplicate: duplicate(() => existingRoleIds, true),
      minLength: minLength(4),
      maxLength: maxLength(64),
      required
    }
  }
};

@Component({
  components: components,
  mixins: [validationMixin],
  props: props,
  template: require("./create.html"),
  validations: validations
})
export class CreateRole extends Vue {
  private svc: ManageRoleService;

  $common: ICommonOptions;
  $route: IRouteMixinData;
  $store: Store<{}>;
  $v: Vuelidate<any>;
  allowedValuesPattern: string;
  availableClaims: ISecurityClaim[] = [];
  claimsValid: boolean = true;
  id: string = this.id;
  init: boolean = false;
  parentRoles: IVueSelectOption[];
  role: IRole = DefaultRole;
  selectedParentRole: IVueSelectOption = null;
  systemRoles: IRole[] = [];

  created() {
    let svc = (this.svc = new ManageRoleService(this.$store));

    if (this.id) this.svc.cloneRole(this.id).then(this.onGetRole);
  }

  get allowSubmit() {
    return this.$v.role.$anyDirty && this.claimsValid && !this.$v.role.$invalid;
  }

  submit() {
    let onSuccess = (value: IRole) => {
      if (value) {
        let msg: IStatusBarData = {
          text: "admin.role.created",
          title: "dict.success",
          messageTypeId: PayloadMessageTypes.success
        };

        this.$store.dispatch(StoreTypes.updateStatusBar, msg);

        this.$nextTick().then(() => {
          this.$router.push({ name: "viewRole", params: { id: value.roleId } });
        });
      }
    };

    this.svc.createRole(this.role).then(onSuccess);
  }

  onClaimsChange(value: any[]) {
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

    existingRoleIds = value.existingRoles.map(o => o.roleId);
    existingRoleNames = value.existingRoles.map(o => o.name);

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
    let key = "dict.create";
    if (this.$te(key)) return `${this.$t("dict.role")} : ${this.$t(key)}`;
    else return this.$t("dict.role");
  }
}
