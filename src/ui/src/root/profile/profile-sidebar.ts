import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';

@Component({
  template: require('./profile-sidebar.html')
})
export class ProfileSidebar extends Vue {

  $t: Formatter
}

export default ProfileSidebar;