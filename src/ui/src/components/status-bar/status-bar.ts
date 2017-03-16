import Vue = require('vue');
import { Store } from 'vuex';
import { State } from 'vuex-class';
import Component from 'vue-class-component';
import { Formatter, KeypathChecker } from 'vue-i18n';
import { PayloadMessageTypes, TokenHelper } from '../../common';
import { IPayload, IStatusBarData } from '../../model';
import { ICommonState, StoreTypes } from '../../store';
import './status-bar.scss';

@Component({
  name: 'StatusBar',
  template: `
    <transition name="fade">
      <div v-if="status.messageTypeId" @click.prevent="onStatusBarClick()" class="status-bar" :class="alertClass" role="alert">
         <span class="status-title">{{statusTitle}}</span><span class="status-text">{{statusText}}</span>
      </div>
    </transition>`,
  props: {
    clearAfter: Number,
    localePrefix: String
  }
})
export class StatusBar extends Vue {

  @State((state: { common: ICommonState }) => state.common.statusBar) status: IStatusBarData;

  private static handle: number = null;

  created() {

    let resetAlert = (timeout: number) => {

      this.timeoutHandle = window.setTimeout(() => {
        this.$store.dispatch(StoreTypes.updateStatusBar, null);
        this.clearTimeoutHandle();

      }, timeout);
    };

    this.$store.subscribe((mutation, state: { common: ICommonState }) => {

      if (mutation.type === StoreTypes.updateStatusBar) {

        let timeout = state.common.statusBar.timeout || this.alertTimeout;

        if (state.common.statusBar.messageTypeId && timeout !== -1 && !this.timeoutHandle) {
          resetAlert(timeout);
        }
      }
    })
  }

  clearTimeoutHandle(): void {

    if (this.timeoutHandle) {
      window.clearTimeout(this.timeoutHandle);
      this.timeoutHandle = null;
    }
  };

  onStatusBarClick(): void {

    if (!this.status.uri) {

      this.clearTimeoutHandle();
      this.$store.dispatch(StoreTypes.updateStatusBar, null);
    }
    else
      window.location.href = this.status.uri;

  }

  clearAfter: number = this.clearAfter;
  localePrefix: string = this.localePrefix;

  public get statusText(): string {

    let key = this.getLocaleKey(this.status.text);

    return this.$te(key) ? this.$t(key) : this.status.text;
  }

  public get statusTitle(): string {

    let key = this.getLocaleKey(this.status.title);

    return this.$te(key) ? this.$t(key) : null;
  }

  public get timeoutHandle(): number {
    return StatusBar.handle;
  }

  public set timeoutHandle(value: number) {
    StatusBar.handle = value;
  }

  private getLocaleKey(value: string): string {

    return value && value.indexOf('.') === -1 ? this.localePrefix + '.' + value : value;
  }

  private get alertTimeout(): number {

    let timeout: number = this.clearAfter;
    timeout = timeout || 0;

    if (timeout > 0)
      timeout = timeout > 1000 ? timeout : timeout * 1000;
    else
      timeout = -1;

    return timeout;
  }

  public get alertClass(): string {

    let subClass = '';

    switch (this.status.messageTypeId) {
      case PayloadMessageTypes.error: subClass = 'alert-danger'; break;
      case PayloadMessageTypes.failure: subClass = 'alert-warning'; break;
      case PayloadMessageTypes.info: subClass = 'alert-info'; break;
      case PayloadMessageTypes.success: subClass = 'alert-success'; break;
      case PayloadMessageTypes.warning: subClass = 'alert-warning'; break;
    }

    return 'alert ' + subClass;
  }

  $store: Store<{}>;

  $t: Formatter;

  $te: KeypathChecker;

}

export default StatusBar;

