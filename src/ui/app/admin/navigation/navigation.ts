import * as Vue from 'vue';
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import * as i18n from 'vue-i18n';
import { SupportedLocales } from '../../locales';
import { IUser } from '../../model';
import { Debounce, GlobalConfig } from '../../common';
import { AuthenticationService } from '../../services';
import { IRouterMixin, IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { RouteNames } from '../routes'
import { IAdminStoreState, AdminStoreTypes } from '../store';

@Component({
  template: require('./navigation.html')
})
export class AreaNavigation extends Vue implements IRouterMixin {

  private auth: AuthenticationService;

  @State((state: IAdminStoreState) => state.common.user) user: IUser;

  changeLocale(locale: string, e: Event) {

    if (locale !== this.$i18n.locale) {
      this.$store.dispatch(AdminStoreTypes.common.updateLocale, locale)
        .then(() => {
          this.$i18n.locale = locale;
          this.$router.replace(this.$route.path);
        })
        .catch(e => this.$store.dispatch(AdminStoreTypes.common.updateStatusBar, e));
    }
  }

  created() {
    this.auth = new AuthenticationService(this.$store);
  }

  get locales() {
    return SupportedLocales;
  }

  logout(e: Event) {

    this.auth.logout()
      .then(user => {
        this.$store.dispatch(AdminStoreTypes.common.updateUser, user);
      })
      .then(() => {
        let back = window.history.length;
        window.history.go(back);
        window.location.replace(GlobalConfig.uri.site);
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

    Debounce(onSubmitSearch, 500)();
  }

  $route: IRouteMixinData;

  $router: IRouterMixinData;

  $store: Store<IAdminStoreState>;

  searchOptions = {
    searchText: '',
    showInput: false
  }
}