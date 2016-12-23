import Vue = require('vue');
import Component from 'vue-class-component';

@Component({
  template: require('./notfound.html')
})
export class PageNotFound extends Vue {
}

export default PageNotFound;