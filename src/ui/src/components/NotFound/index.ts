import Vue = require('vue');
import Component from 'vue-class-component';

@Component({
  template: require('./notfound.html')
})
class NotFound extends Vue {
}

export default NotFound;