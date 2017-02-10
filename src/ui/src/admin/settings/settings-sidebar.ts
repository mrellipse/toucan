import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';

@Component({
  template: require('./settings-sidebar.html')
})
export class SiteSettingsSidebar extends Vue {

  $t: Formatter
  
}

export default SiteSettingsSidebar;