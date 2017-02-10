import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';

@Component({
  template: require('./reports-sidebar.html')
})
export class SiteReportsSidebar extends Vue {

  $t: Formatter
}

export default SiteReportsSidebar;