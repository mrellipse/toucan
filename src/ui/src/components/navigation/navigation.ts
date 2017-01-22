import Vue = require('vue');
import Component from 'vue-class-component';
import { IUser } from '../../model';
import { events } from '../../main';
import { Formatter } from 'vue-i18n';
import { AuthMixin, IAuthMixin, IAuthMixinData } from '../../mixins/mixin-auth';

@Component({
  template: require('./navigation.html'),
  mixins: [AuthMixin]
})
export class Navigation extends Vue implements IAuthMixin {

  $a: IAuthMixinData

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
    this.$a.logout();
  }

  $t: Formatter

  user: IUser = { authenticated: false };
}

export default Navigation;