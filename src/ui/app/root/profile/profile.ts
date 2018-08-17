import Vue from 'vue';
import Component from 'vue-class-component';
import { State } from 'vuex-class';
import { ContentService } from '../services';
import { IUser } from '../../model';
import { IRootStoreState } from '../store';
import { Store } from 'vuex';

@Component({
  template: require('./profile.html')
})
export class Profile extends Vue {

  @State((state: IRootStoreState) => state.common.user) user: IUser;
  
  content: string = '';

  created(): void {
    let svc = new ContentService(this.$store);
    
    svc.secureUserContent()
      .then(value => this.content = value);
  }

  $store: Store<IRootStoreState>;
}