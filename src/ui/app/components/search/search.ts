import Vue from 'vue';
import Component from 'vue-class-component';
import { IRouteMixinData } from '../../mixins/mixin-router';

@Component({
  template: require('./search.html')
})
export class Search extends Vue {

  get searchText() {
    return this.$route.params['searchText'];
  }

  $route: IRouteMixinData;
}