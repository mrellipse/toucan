import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { AuthenticationHelper, IClaimsHelper } from '../../helpers';

@Component({
  template: require('./home.html')
})
export class Home extends Vue {

  private auth: AuthenticationHelper;

  created() {

    this.auth = new AuthenticationHelper();

  }

  get authenticated() {

    return this.auth.user.authenticated;

  }

  get displayName() {

    return this.auth.user.displayName;

  }

  $t: Formatter
}

export default Home;