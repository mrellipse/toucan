import Vue = require('vue');
import { Store } from 'vuex';
import Component from 'vue-class-component';
import { Formatter } from 'vue-i18n';
import { ManageUserService } from './user-service';
import { IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { ICommonOptions } from '../../plugins';
import { ISearchResult, IStatusBarData, IUser } from '../../model';
import { StoreTypes } from '../../store';
import { Switch } from '../../components';

import './users.scss';

@Component({
  template: require('./user-list.html'),
  components: {
    check: Switch
  },
  props: ['page', 'pageSize']
})
export class ManageUserList extends Vue {

  private svc: ManageUserService;
  private searchResults: ISearchResult<IUser> = null;

  created() {

    let svc = this.svc = new ManageUserService();

    this.search();
  }

  private search() {
    let onSuccess = (value: ISearchResult<IUser>) => {

      this.searchResults = value;

      let page = value.page;
      let pageSize = value.pageSize;
      let total = value.total;

      this.start = 1 + (page > 1 ? (page - 1) * pageSize : 0);
      this.end = (page * pageSize) >= total ? total : (page * pageSize);
    };

    this.$common.exec(this.svc.search(this.currentPage, this.currentPageSize))
      .then(onSuccess);
  }

  updateUserStatus(user: IUser) {
    let data = {
      username: user.username,
      enabled: user.enabled,
      verified: user.verified
    };

    this.$common.exec(this.svc.updateUserStatus(data));
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
  start: number = 1;
  end: number = 5;

  public get total(): number {
    return this.searchResults ? this.searchResults.total : null;
  }

  public get users(): IUser[] {
    return this.searchResults ? this.searchResults.items : [];
  }
  $common: ICommonOptions;
  $route: IRouteMixinData;
  $router: IRouterMixinData;
  $store: Store<{}>;
  $t: Formatter;
}

export default ManageUserList;