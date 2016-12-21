import Vue = require('vue');
import Component from 'vue-class-component';

@Component({
  template: require('./navigation.html')
})
class Navigation extends Vue {
}

export default Navigation;