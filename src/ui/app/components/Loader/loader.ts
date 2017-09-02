import * as Vue from 'vue';
import Component from 'vue-class-component';
import './loader.scss';

@Component({
  template: `<div class="row">
    <div class="col loader"></div>
  </div>`
})
export class Loader extends Vue {
}

