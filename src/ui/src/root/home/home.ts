import Vue = require('vue');
import { Store } from 'vuex';
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import { AuthenticationService, IClaimsHelper } from '../../services';
import { State } from 'vuex-class';
import { IRootStoreState, RootStoreTypes } from '../store';

// URL and endpoint constants
const API_URL = 'https://localhost:5000/api/';
const RIKER_IPSUM = API_URL + 'content/rikeripsum';

@Component({
  template: require('./home.html')
})
export class Home extends Vue {

  @State((state: IRootStoreState) => state.common.user.authenticated) authenticated: boolean

  @State((state: IRootStoreState) => state.common.user.displayName) displayName: string

  @State((state: IRootStoreState) => state.common.isLoading) isLoading: boolean

  @State((state: IRootStoreState) => state.secureContent) secureContent: string

  created() {

    if (this.authenticated) {

      let onError = (e: any) => {

        this.$store.dispatch(RootStoreTypes.secureContent, '');
        this.$store.dispatch(RootStoreTypes.common.loadingState, false);
      }

      let onSuccess = (res: AxiosResponse) => {

        let payload: string = res.data;

        this.$store.dispatch(RootStoreTypes.secureContent, payload);
        this.$store.dispatch(RootStoreTypes.common.loadingState, false);
      }

      this.$store.dispatch(RootStoreTypes.common.loadingState, true);

      return Axios.get(RIKER_IPSUM)
        .then(onSuccess)
        .catch(onError);
    }

  }

  $store: Store<IRootStoreState>;

  $t: Formatter;
}

export default Home;