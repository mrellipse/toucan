import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';

@Component({
  template: require('./dashboard.html')
})
export class AreaDashboard extends Vue {

  $t: Formatter
}

export default AreaDashboard;