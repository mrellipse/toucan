import Vue = require('vue');
import Component from 'vue-class-component';
import { State } from 'vuex-class';
import { Formatter } from 'vue-i18n';
import { AuthenticationService } from '../../services';
import { IUser } from '../../model';
import { IRootStoreState } from '../store';

@Component({
  template: require('./profile.html')
})
export class Profile extends Vue {

  @State((state: IRootStoreState) => state.common.user) user: IUser;

  $t: Formatter
}

export default Profile;