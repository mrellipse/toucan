import Vue from 'vue';
import { Store } from 'vuex';
import Component from 'vue-class-component';
import * as i18n from 'vue-i18n';
import { VueModelDate } from 'vue-model-date/lib-esm';
import { ManageUserService } from './user-service';
import { IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { ICommonOptions } from '../../plugins';
import { ISearchResult, IStatusBarData, IUser } from '../../model';
import { ICommonState, StoreTypes } from '../../store';
import { Toggle } from '../../components';

import './users.scss';
import { AdminStoreTypes } from '../store';
import { PayloadMessageTypes } from '../../common';

@Component({
  template: require('./user-list.html'),
  components: {
    toggle: Toggle
  },
  directives: {
    'modelDate': VueModelDate
  },
  props: ['page', 'pageSize']
})
export class ManageUserList extends Vue {

  private svc: ManageUserService;
  private searchResults: ISearchResult<IUser> = null;

  created() {

    this.svc = new ManageUserService(this.$store);

    let unwatch = this.$watch(() => this.registeredAfter, (oldValue, newValue) => {
      
      let msg: IStatusBarData = {
        text: 'dict.warnings.stub',
        title: 'dict.notImplemented',
        messageTypeId: PayloadMessageTypes.warning
      }

      this.$store.dispatch(AdminStoreTypes.common.updateStatusBar, msg)
        .then(() => unwatch());
    });

    this.search();
  }

  private search() {

    let onSuccess = (value: ISearchResult<IUser>) => {

      if (value) {
        this.searchResults = value;

        let page = value.page;
        let pageSize = value.pageSize;
        let total = value.total;

        this.start = 1 + (page > 1 ? (page - 1) * pageSize : 0);
        this.end = (page * pageSize) >= total ? total : (page * pageSize);
      }
    };

    this.svc.search(this.currentPage, this.currentPageSize)
      .then(onSuccess);
  }

  updateUserStatus(user: IUser) {

    let data = {
      username: user.username,
      enabled: user.enabled
    };

    this.svc.updateUserStatus(data);
  }

  public get onText() {
    return this.$t('dict.yes');
  }

  public get offText() {
    return this.$t('dict.no');
  }

  navigate(forward: boolean) {

    if (forward)
      this.currentPage++;
    else
      this.currentPage--;

    this.search();
  }

  currentPage: number = Object.assign((<any>this).page);
  currentPageSize: number = Object.assign((<any>this).pageSize);
  registeredAfter: Date = null;
  start: number = 1;
  end: number = 5;

  public get total(): number {
    return this.searchResults ? this.searchResults.total : null;
  }

  public get users(): IUser[] {
    return this.searchResults ? this.searchResults.items : [];
  }

  $route: IRouteMixinData;
  $router: IRouterMixinData;
  $store: Store<{ common: ICommonState }>;
}