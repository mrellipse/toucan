import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';

@Component({
  template: require('./home.html')
})
export class Home extends Vue {

  $t: Formatter
}

export default Home;