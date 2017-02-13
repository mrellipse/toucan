import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { AuthenticationHelper, IClaimsHelper } from '../../helpers';
import { State } from 'vuex-class';
import { IRootStoreState  } from '../store';

@Component({
  template: require('./home.html')
})
export class Home extends Vue {

  @State((state:IRootStoreState) => state.common.user.authenticated) authenticated: boolean

  @State((state:IRootStoreState) => state.common.user.displayName) displayName: string

  created() {

  }

  $t: Formatter
}

export default Home;