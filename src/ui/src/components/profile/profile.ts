import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { AuthenticationHelper } from '../../helpers';
import { IUser } from '../../model';

@Component({
  template: require('./profile.html')
})
export class Profile extends Vue {

  created() {
    this.updateUser();
  }

  private updateUser() {
    let auth = new AuthenticationHelper();
    this.user = Object.assign({}, auth.user);
  }

  user: IUser = { authenticated: false, email: null, name: null, username: null, roles: [] };

  $t: Formatter
}

export default Profile;