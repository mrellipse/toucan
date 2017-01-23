import Vue = require('vue');
import Component from 'vue-class-component';
import { IUser } from '../../model';
import { events } from '../../main';
import { Formatter } from 'vue-i18n';
import { SupportedLocales } from '../../locales';
import { AuthMixin, IAuthMixin, IAuthMixinData } from '../../mixins/mixin-auth';
import { IRouterMixin, IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';

@Component({
  template: require('./navigation.html'),
  mixins: [AuthMixin]
})
export class Navigation extends Vue implements IAuthMixin, IRouterMixin {

  $a: IAuthMixinData

  changeLocale(lang: string, e: Event) {
    if (e)
      e.preventDefault();

    if (lang !== this.$lang) {
      (<any>Vue.config).lang = lang;
      events.$emit(events.global.localeChange, lang);
    }

  }

  created() {

    let user = this.user;

    events.$on(events.global.login, (data: IUser) => {
      Object.assign(user, data);
    });

    events.$on(events.global.logout, (data: IUser) => {
      Object.assign(user, data);
    });

    events.$on(events.global.localeChange, (lang: string) => {

      this.$router.replace(this.$route.path);
    });

  }

  get locales() {
    return SupportedLocales;
  }

  logout(e: Event) {
    this.$a.logout();
  }

  user: IUser = { authenticated: false };

  $lang: string

  $route: IRouteMixinData;

  $router: IRouterMixinData;

  $t: Formatter

}

export default Navigation;