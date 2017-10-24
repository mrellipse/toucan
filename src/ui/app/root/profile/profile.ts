import Vue from 'vue';
import Component from 'vue-class-component';
import { State } from 'vuex-class';
import { AuthenticationService } from '../../services';
import { IUser } from '../../model';
import { IRootStoreState } from '../store';

@Component({
  template: require('./profile.html')
})
export class Profile extends Vue {

  @State((state: IRootStoreState) => state.common.user) user: IUser;
  
}