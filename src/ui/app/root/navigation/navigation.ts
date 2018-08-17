import Vue from 'vue';
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import { Debounce, GlobalConfig } from '../../common';
import { Autocomplete } from '../../components';
import { SupportedLocales, SupportedTimeZones } from '../../locales';
import { IUser, SecurityRoleClaims } from '../../model';
import { AuthenticationService, ProfileService } from '../../services';
import { IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { RouteNames } from '../routes';
import { IRootStoreState, RootStoreTypes } from '../store';

import './navigation.scss';

@Component({
  components: { autocomplete: Autocomplete },
  template: require('./navigation.html')
})
export class AreaNavigation extends Vue {

  private auth: AuthenticationService;

  private profile: ProfileService;

  @State((state: IRootStoreState) => state.common.user) user: IUser;

  changeLocale(locale: string, e: Event) {

    if (locale !== this.$i18n.locale) {
      // user-options-plugin mixin watches for locale changes, and will invoke logic to load resource strings
      this.$store.dispatch(RootStoreTypes.common.updateLocale, locale);
    }
  }

  created() {

    this.auth = new AuthenticationService(this.$store);
    this.profile = new ProfileService();
  }

  onTimeZoneChange(timeZoneId: string) {

    if (timeZoneId && this.timeZones.find(o => o.key == timeZoneId) && timeZoneId != this.user.timeZoneId) {
      this.$store.dispatch(RootStoreTypes.common.updateTimeZone, timeZoneId);
      this.showTimeZones = false;
    }
  }

  beforeMount() {

    this.activeTimeZoneId = this.user.timeZoneId;
  }

  get isInAdminRole() {
    
    return this.user.authenticated && this.auth.satisfies(this.user, [SecurityRoleClaims.Admin]);
  }

  get locales() {
    return SupportedLocales;
  }

  logout(e: Event) {

    this.auth.logout()
      .then(user => {
        this.$store.dispatch(RootStoreTypes.common.updateUser, user);
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

  toggleTimeZoneInput(display: boolean = null) {

    display = display || !this.showTimeZones;
    this.showTimeZones = display;
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

  $store: Store<IRootStoreState>;

  searchOptions = {
    searchText: '',
    showInput: false
  }

  showTimeZones: boolean = false;

  get activeCulture() {
    return this.user.cultureName;
  }

  get activeTimeZoneId() {

    return this.user.timeZoneId;
  }

  set activeTimeZoneId(value: string) {

    if (value && this.timeZones.find(o => o.key == value) && value != this.user.timeZoneId) {
      this.$store.dispatch(RootStoreTypes.common.updateTimeZone, value);
      this.showTimeZones = false;
    }
  }

  get timeZones() {
    return SupportedTimeZones;
  }
}