import Vue = require('vue');
import Component from 'vue-class-component';
import './loader.scss';

@Component({
  template: '<div class="loader"></div>'
})
export class Loader extends Vue {
}

export default Loader;

