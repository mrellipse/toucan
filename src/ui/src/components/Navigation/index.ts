import Vue = require('vue');
import Component from 'vue-class-component';

@Component({
  template: require('./navigation.html')
})
export class Navigation extends Vue {
}

export default Navigation;