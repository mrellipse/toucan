import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { AreaSidebar } from '../sidebar/sidebar';

@Component({
  components: {
    AreaSidebar: AreaSidebar
  },
  template: require('./layout.html')
})
export class AreaLayout extends Vue {
  $t: Formatter
}

export default AreaLayout;