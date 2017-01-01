import Vue = require('vue');
import Component from 'vue-class-component';
import { IUser } from '../../model';
import { events, auth } from '../../main';

@Component({
  template: require('./navigation.html')
})
export class Navigation extends Vue {

  user: IUser = { authenticated: false };

  created() {

    let user = this.user;
    
    events.$on(events.global.login, (data: IUser) => {
      Object.assign(user, data);
    });

    events.$on(events.global.logout, (data: IUser) => {
      Object.assign(user, data);
    });

  }

  logout(e: Event) {
    auth.logout();
  }
}

export default Navigation;