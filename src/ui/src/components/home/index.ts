import Vue = require('vue');
import Component from 'vue-class-component';

@Component({
  template: require('./home.html')
})
export class Home extends Vue {
}

export default Home;