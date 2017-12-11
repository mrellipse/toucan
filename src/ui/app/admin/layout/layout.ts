import Vue from 'vue';
import Component from 'vue-class-component';
import { AreaSidebar } from '../sidebar/sidebar';
import { TokenExpiry } from '../../components/';

@Component({
  components: { AreaSidebar, TokenExpiry },
  template: require('./layout.html')
})
export class AreaLayout extends Vue {

}