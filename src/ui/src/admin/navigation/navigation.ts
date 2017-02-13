import Vue = require('vue');
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import { Formatter } from 'vue-i18n';
import { SupportedLocales } from '../../locales';
import { IUser } from '../../model';
import { debounce } from '../../helpers/debounce';
import { AuthenticationHelper } from '../../helpers';
import { IRouterMixin, IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { RouteNames } from '../routes/route-names'
import { IAdminStoreState, RootStoreTypes } from '../store';

@Component({
  template: require('./navigation.html')
})
export class AreaNavigation extends Vue implements IRouterMixin {

  private auth: AuthenticationHelper;

  @State((state: IAdminStoreState) => state.common.user) user: IUser;

  changeLocale(lang: string, e: Event) {

    if (lang !== this.$lang) {
      (<any>Vue.config).lang = lang;
      this.$router.replace(this.$route.path);
    }

  }

  created() {
    this.auth = new AuthenticationHelper();
  }

  get locales() {
    return SupportedLocales;
  }

  logout(e: Event) {

    let user = this.auth.logout();
    this.$store.dispatch(RootStoreTypes.common.updateUser, user).then(() => 
    {
      this.$router.push('/');
    });
    
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

  $store: Store<IAdminStoreState>;

  $t: Formatter

  searchOptions = {
    searchText: '',
    showInput: false
  }

}

export default AreaNavigation;