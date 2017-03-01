import Vue = require('vue');
import Component from 'vue-class-component';

@Component({
  template: require('./forbidden.html')
})
export class PageForbidden extends Vue {
}

export default PageForbidden;