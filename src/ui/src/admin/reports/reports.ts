import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';

@Component({
  template: require('./reports.html')
})
export class SiteReports extends Vue {

  $t: Formatter
}

export default SiteReports;