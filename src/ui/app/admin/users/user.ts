import Vue from "vue";
import { Store } from "vuex";
import Component from "vue-class-component";
import { Vuelidate, validationMixin } from "vuelidate";
import { required, minLength } from "vuelidate/lib/validators";
import { ManageUserService, IGetUserPayload } from "./user-service";
import { IRouteMixinData } from "../../mixins/mixin-router";
import { PayloadMessageTypes } from "../../common";
import { IStatusBarData, IUser, DefaultUser, KeyValue } from "../../model";
import { StoreTypes } from "../../store";
import { SupportedLocales, SupportedTimeZones } from "../../locales";
import { Toggle } from "../../components";

import "./users.scss";
import { ICommonOptions, IVueSelectOption } from "../../plugins";

let validations = {
  user: {
    displayName: {
      required,
      minLength: minLength(2)
    },
    roles: {
      required
    },
    cultureName: {
      required
    },
    timeZoneId: {
      required
    }
  }
};

@Component({
  components: {
    check: Toggle
  },
  mixins: [validationMixin],
  props: ["id"],
  template: require("./user.html"),
  validations: validations
})
export class ManageUser extends Vue {
  private svc: ManageUserService;

  created() {
    let svc = (this.svc = new ManageUserService(this.$store));

    this.supportedCultures = this.$common.mapArrayToOptions(SupportedLocales);
    this.supportedTimeZones = this.$common.mapKeyValuesToOptions(
      SupportedTimeZones
    );

    if (this.id) {
      this.svc.getUser(this.id).then(this.onGetUser);
    }
  }

  get allowSubmit() {
    return (
      !this.$v.$invalid &&
      (this.$v.user.displayName.$dirty ||
        this.$v.user.roles.$dirty ||
        this.$v.user.timeZoneId.$dirty ||
        this.$v.user.cultureName.$dirty)
    );
  }

  isInRole(roleId: string) {
    return this.user
      ? this.user.roles.find(o => o === roleId) !== undefined
      : false;
  }

  onGetUser(value: IGetUserPayload) {
    this.user = value.user;
    this.availableRoles = this.$common.mapKeysToOptions(value.availableRoles);

    this.selectedCulture = this.supportedCultures.find(
      o => o.id == this.user.cultureName
    );
    this.selectedRole = this.availableRoles.find(
      o => o.id == this.user.roles[0]
    );
    this.selectedTimeZone = this.supportedTimeZones.find(
      o => o.id == this.user.timeZoneId
    );

    this.$watch(() => this.selectedCulture, this.onCultureChanged);
    this.$watch(() => this.selectedRole, this.onRoleChanged);
    this.$watch(() => this.selectedTimeZone, this.onTimeZoneChanged);

    this.init = true;
  }

  onCultureChanged(newValue: IVueSelectOption) {
    this.user.cultureName = newValue ? newValue.id : null;
    this.$v.user.cultureName.$touch();
  }

  onRoleChanged(newValue: IVueSelectOption) {
    this.user.roles[0] = newValue ? newValue.id : null;
    this.$v.user.roles.$touch();
  }

  onTimeZoneChanged(newValue: IVueSelectOption) {
    this.user.timeZoneId = newValue ? newValue.id : null;
    this.$v.user.timeZoneId.$touch();
  }

  submit() {
    let onSuccess = (value: IUser) => {
      if (value) {
        let msg: IStatusBarData = {
          text: "user.updated",
          title: "dict.success",
          messageTypeId: PayloadMessageTypes.success
        };

        this.$store.dispatch(StoreTypes.updateStatusBar, msg);
      }
    };

    this.svc.updateUser(this.user).then(onSuccess);
  }

  updateUserStatus(user: IUser) {
    let data = {
      username: user.username,
      enabled: user.enabled
    };

    this.svc.updateUserStatus(data);
  }

  availableRoles: IVueSelectOption[] = [];
  id: number | string = this.id;
  selectedCulture: IVueSelectOption = null;
  selectedRole: IVueSelectOption = null;
  selectedTimeZone: IVueSelectOption = null;
  supportedCultures: IVueSelectOption[] = [];
  supportedTimeZones: IVueSelectOption[] = [];

  locales: {
    en: { userUpdated: "User updated" };
    fr: { userUpdated: "Mise à jour de l'utilisateur" };
  };

  public get onText() {
    return this.$t("dict.yes");
  }

  public get offText() {
    return this.$t("dict.no");
  }

  $common: ICommonOptions;
  $route: IRouteMixinData;
  $store: Store<{}>;
  init: boolean = false;
  user: IUser = DefaultUser;
  $v: Vuelidate<any>;
}