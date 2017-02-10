import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';

@Component({
  template: require('./settings.html')
})
export class SiteSettings extends Vue {

  $t: Formatter
}

export default SiteSettings;