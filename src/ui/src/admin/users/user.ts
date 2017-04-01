import Vue = require('vue');
import { Store } from 'vuex';
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { Vuelidate, validationMixin } from 'vuelidate';
import { required, minLength } from 'vuelidate/lib/validators';
import { ManageUserService } from './user-service';
import { IRouteMixinData } from '../../mixins/mixin-router';
import { PayloadMessageTypes, TokenHelper } from '../../common';
import { IKeyValueList, ISearchResult, IStatusBarData, IUser } from '../../model';
import { StoreTypes } from '../../store';
import { DropDownSelect, Switch } from '../../components';

import './users.scss';

let validations = {
  user: {
    displayName: {
      required,
      minLength: minLength(2)
    },
    roles: {
      required
    }
  }
};

@Component({
  components: {
    dropDown: DropDownSelect,
    check: Switch
  },
  mixins: [validationMixin],
  props: ['id'],
  template: require('./user.html'),
  validations: validations
})
export class ManageUser extends Vue {
  private svc: ManageUserService;
  created() {
    let svc = this.svc = new ManageUserService();

    if (this.id) {

      this.$store.dispatch(StoreTypes.loadingState, true)
        .then(() => this.svc.getUser(this.id))
        .then((value) => {
          this.user = value.user
          this.availableRoles = value.availableRoles;
        })
        .then(() => this.$store.dispatch(StoreTypes.loadingState, false))
        .catch(e => this.$store.dispatch(StoreTypes.updateStatusBar, e));
    }
  }

  get allowSubmit() {
    return !this.$v.$invalid && (this.$v.user.displayName.$dirty || this.$v.user.roles.$dirty);
  }
  submit() {

    let onSuccess = (value: IUser) => {

      let msg: IStatusBarData = {
        text: 'user.updated',
        title: 'dict.success',
        messageTypeId: PayloadMessageTypes.success
      }

      this.$store.dispatch(StoreTypes.updateStatusBar, msg);
    };

    this.svc.updateUser(this.user)
      .then(onSuccess)
      .catch(e => this.$store.dispatch(StoreTypes.updateStatusBar, e));
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
  public get onText() {
    return this.$t('dict.yes');
  }

  public get offText() {
    return this.$t('dict.no');
  }

  updateUserStatus(user: IUser) {
    let data = {
      username: user.username,
      enabled: user.enabled,
      verified: user.verified
    };

    this.svc.updateUserStatus(data)
      .catch(e => this.$store.dispatch(StoreTypes.updateStatusBar, e));
  }

  $route: IRouteMixinData;
  $store: Store<{}>;
  $t: Formatter;
  user: IUser = null;
  $v: Vuelidate<any>
}

export default ManageUser;