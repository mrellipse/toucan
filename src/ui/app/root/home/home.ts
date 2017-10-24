import Vue from 'vue';
import { Store } from 'vuex';
import Component from 'vue-class-component';
import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import { State } from 'vuex-class';
import { ICommonOptions } from '../../plugins';
import { AuthenticationService, IClaimsHelper } from '../../services';
import { ContentService } from '../services';
import { IPayload } from '../../model';
import { IRootStoreState, RootStoreTypes } from '../store';

require('./home.scss');

@Component({
  template: require('./home.html')
})
export class Home extends Vue {

  private svc: ContentService = null;

  @State((state: IRootStoreState) => state.common.user.authenticated) authenticated: boolean

  @State((state: IRootStoreState) => state.common.user.displayName) displayName: string

  @State((state: IRootStoreState) => state.common.isLoading) isLoading: boolean

  @State((state: IRootStoreState) => state.apiCallContent) apiCallContent: string

  created() {

    this.svc = new ContentService(this.$store);

    let onFulfilled = () => this.init = true;

    this.svc.rikerIpsum()
      .then(onFulfilled);
  }

  init: Boolean = false;

  $common: ICommonOptions;

  $store: Store<IRootStoreState>;

  get siteLogoAlt() {
    return this.$t('dict.logo');
  }
}