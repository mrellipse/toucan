import Vue = require('vue');
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { SupportedLocales } from '../../locales';
import { IUser } from '../../model';
import { EventBus } from '../../events';
import { debounce } from '../../helpers/debounce';
import { AuthenticationHelper } from '../../helpers';
import { RouteNames } from '../routes/route-names'
import { IRouterMixin, IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';

@Component({
  template: require('./navigation.html')
})
export class AreaNavigation extends Vue implements IRouterMixin {

  private auth: AuthenticationHelper;

  changeLocale(lang: string, e: Event) {

    if (lang !== this.$lang) {
      (<any>Vue.config).lang = lang;
      EventBus.$emit(EventBus.global.localeChange, lang);
      this.$router.replace(this.$route.path);
    }

  }

  created() {
    this.auth = new AuthenticationHelper();

    if (this.auth.user.authenticated)
      this.user = Object.assign({}, this.auth.user);

    let user = this.user;

    EventBus.$on(EventBus.global.logout, (data: IUser) => {
      this.auth.clearUserData(this.user);
    });

  }

  get locales() {
    return SupportedLocales;
  }

  logout(e: Event) {
    this.auth.logout();
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

  $lang: string

  $route: IRouteMixinData;

  $router: IRouterMixinData;

  $t: Formatter

  searchOptions: ISearchOptions = {
    searchText: '',
    showInput: false
  }

  user: IUser = { authenticated: false, username: null, roles: [] };
}

interface ISearchOptions {
  searchText: string;
  showInput: boolean;
}

export default AreaNavigation;