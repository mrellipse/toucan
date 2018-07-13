import Vue from 'vue';
import { Store } from 'vuex';
import Component from 'vue-class-component';
import * as i18n from 'vue-i18n';
import { Vuelidate, validationMixin } from 'vuelidate';
import { required, minLength } from 'vuelidate/lib/validators';
import { ManageUserService } from './user-service';
import { ICommonOptions } from '../../plugins';
import { IRouteMixinData } from '../../mixins/mixin-router';
import { PayloadMessageTypes, TokenHelper } from '../../common';
import { IKeyValueList, ISearchResult, IStatusBarData, IUser, KeyValue } from '../../model';
import { StoreTypes } from '../../store';
import { SupportedLocales, SupportedTimeZones } from '../../locales';
import { DropDownSelect, Toggle } from '../../components';

import './users.scss';

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
    dropDown: DropDownSelect,
    check: Toggle
  },
  mixins: [validationMixin],
  props: ['id'],
  template: require('./user.html'),
  validations: validations
})
export class ManageUser extends Vue {
  private svc: ManageUserService;

  created() {

    let svc = this.svc = new ManageUserService(this.$store);

    if (this.id) {

      let onFulfilled = (value) => {
        this.user = value.user
        this.availableRoles = value.availableRoles;
        this.init = true;
      };

      this.svc.getUser(this.id)
        .then(onFulfilled);
    }
  }

  get allowSubmit() {
    return !this.$v.$invalid && (this.$v.user.displayName.$dirty || this.$v.user.roles.$dirty || this.$v.user.timeZoneId.$dirty || this.$v.user.cultureName.$dirty);
  }

  submit() {

    let onSuccess = (value: IUser) => {

      if (value) {
        let msg: IStatusBarData = {
          text: 'user.updated',
          title: 'dict.success',
          messageTypeId: PayloadMessageTypes.success
        }

        this.$store.dispatch(StoreTypes.updateStatusBar, msg);
      }
    };

    this.svc.updateUser(this.user)
      .then(onSuccess);
  }

  availableRoles: IKeyValueList<string, string> = [];
  id: number | string = this.id;

  isInRole(roleId: string) {
    return this.user ? this.user.roles.find(o => o === roleId) !== undefined : false;
  }

  locales: {
    en: { userUpdated: 'User updated' },
    fr: { userUpdated: 'Mise à jour de l\'utilisateur' }
  }

  get supportedCultures(): string[] {
    return SupportedLocales;
  }

  get supportedTimeZones(): KeyValue[] {
    return SupportedTimeZones;
  }

  public get onText() {
    return this.$t('dict.yes');
  }

  public get offText() {
    return this.$t('dict.no');
  }

  updateUserStatus(user: IUser) {

    let data = {
      username: user.username,
      enabled: user.enabled
    };

    this.svc.updateUserStatus(data);
  }

  $route: IRouteMixinData;
  $store: Store<{}>;
  init: boolean = false;
  user: IUser = { authenticated: false, cultureName: null, timeZoneId: null, displayName: null, email: null, name: null, username: null, roles: [], verified: false };
  $v: Vuelidate<any>
};