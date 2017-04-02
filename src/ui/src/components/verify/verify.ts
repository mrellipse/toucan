import Vue = require('vue');
import { Store } from 'vuex';
import { RawLocation } from 'vue-router';
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { required } from 'vuelidate/lib/validators';
import { ICommonOptions } from '../../plugins';
import { IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { IStatusBarData, IUser } from '../../model';
import { PayloadMessageTypes } from '../../common';
import { AuthenticationService } from '../../services';
import { StoreTypes } from '../../store';

@Component({
  template: require('./verify.html'),
  validations: {
    verifyCode: {
      required
    }
  }
})
export class Verify extends Vue {

  private auth: AuthenticationService = new AuthenticationService();

  issueVerificationCode() {

    let onSuccess = (code: string) => {
      console.info(`${code}`);
      this.verifyCodeIssued = true;

      let payload: IStatusBarData = {
        text: this.$t('verify.enterCode'),
        messageTypeId: PayloadMessageTypes.success
      };

      this.$store.dispatch(StoreTypes.updateStatusBar, payload)
    };

    this.$common.exec(this.auth.verify())
      .then(onSuccess);
  }

  redeemVerificationCode() {

    let returnUrl: RawLocation = this.$route.query['returnUrl']
    returnUrl = returnUrl || { name: 'home' };

    let onSuccess = (user: IUser) => {

      let payload: IStatusBarData = {
        text: this.$t('verify.codeAck'),
        messageTypeId: PayloadMessageTypes.success,
        timeout: 1500
      };

      this.$store.dispatch(StoreTypes.updateUser, user);
      this.$store.dispatch(StoreTypes.updateStatusBar, payload)
    };

    let onStoreDispatch = (o) => {

      window.setTimeout(() => {
        this.$router.replace(returnUrl);
      }, 2000);
    };

    this.$common.exec(this.auth.redeemVerificationCode(this.verifyCode))
      .then(onSuccess)
      .then(onStoreDispatch)
      ;
  }

  verifyCodeIssued: boolean = false;

  verifyCode: string = null;

  $common: ICommonOptions;

  $route: IRouteMixinData;

  $router: IRouterMixinData;

  $store: Store<{}>;

  $t: Formatter;
}

export default Verify;