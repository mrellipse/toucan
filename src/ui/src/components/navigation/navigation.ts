import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { SupportedLocales } from '../../locales';
import { IUser } from '../../model';
import { events } from '../../main';
import { debounce } from '../../helpers/debounce';
import { AuthMixin, IAuthMixin, IAuthMixinData } from '../../mixins/mixin-auth';
import { IRouterMixin, IRouteMixinData, IRouterMixinData, RouteNames } from '../../mixins/mixin-router';

@Component({
  template: require('./navigation.html'),
  mixins: [AuthMixin]
})
export class Navigation extends Vue implements IAuthMixin, IRouterMixin {

  changeLocale(lang: string, e: Event) {

    if (lang !== this.$lang) {
      (<any>Vue.config).lang = lang;
      events.$emit(events.global.localeChange, lang);
      this.$router.replace(this.$route.path);
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

  }

  get locales() {
    return SupportedLocales;
  }

  logout(e: Event) {
    this.$auth.logout();
  }

  showSearchInput() {
    this.searchOptions.showInput = !this.searchOptions.showInput;
  }

  get searchPageIsActive(): boolean {
    return (this.$route.name === RouteNames.search);
  }

  submitSearch(e: Event) {

    let onSubmitSearch = () => {

      this.searchOptions.showInput = false;

      this.$router.push({
        name: RouteNames.search,
        params: {
          searchText: this.searchOptions.searchText
        }
      });

    }

    debounce(onSubmitSearch, 500)();
  }

  $auth: IAuthMixinData

  $lang: string

  $route: IRouteMixinData;

  $router: IRouterMixinData;

  $t: Formatter

  user: IUser = { authenticated: false };

  searchOptions: ISearchOptions = {
    searchText: '',
    showInput: false
  }

}

interface ISearchOptions {
  searchText: string;
  showInput: boolean;
}

export default Navigation;