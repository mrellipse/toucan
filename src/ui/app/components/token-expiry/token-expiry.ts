import Vue from 'vue';
import Component from 'vue-class-component';
import { State } from 'vuex-class';

import { TokenHelper, PayloadMessageTypes } from '../../common';
import { IRouteMixinData, IRouterMixinData } from '../../mixins/mixin-router';
import { IPayloadMessage, IUser } from '../../model';
import { ICommonState, Store, StoreTypes } from '../../store';
import { AuthenticationService } from '../../services';

@Component({
    props: ['infoTimeout', 'warnTimeout', 'errorTimeout', 'logout'],
    template: `<div></div>`
})
export class TokenExpiry extends Vue {

    private authenticationService: AuthenticationService = null;

    @State((state: { common: ICommonState }) => state.common.user) user: IUser;

    errorTimeout: boolean = this.errorTimeout;
    infoTimeout: number = this.infoTimeout;
    logout: boolean = this.logout;
    warnTimeout: number = this.warnTimeout;

    private infoHandle: number = null;
    private warnHandle: number = null;
    private errorHandle: number = null;

    constructor() {
        super();
    }

    created() {

        this.authenticationService = new AuthenticationService(this.$store);

        if (this.user.exp)
            this.setHandlers(this.user.exp);

        this.$store.watch((state: { common: ICommonState }) => state.common.user.exp, this.tokenExpiryChanged);
    }

    tokenExpiryChanged(tokenExpiresAt: Date) {
        this.clearHandlers();

        if (tokenExpiresAt)
            this.setHandlers(tokenExpiresAt);
    }

    private clearHandlers() {

        if (this.infoHandle)
            window.clearTimeout(this.infoHandle);

        if (this.warnHandle)
            window.clearTimeout(this.warnHandle);

        if (this.errorHandle)
            window.clearTimeout(this.errorHandle);
    }

    private setHandlers(tokenExpiresAt: Date) {

        if (tokenExpiresAt) {

            let ms = tokenExpiresAt.getTime() - new Date().getTime();
            let seconds = (ms / 1000);

            if (this.infoTimeout && seconds > this.infoTimeout)
                this.infoHandle = window.setTimeout(() => this.info(), ms - this.infoTimeout * 1000);

            if (this.warnTimeout && seconds > this.warnTimeout)
                this.warnHandle = window.setTimeout(() => this.warn(), ms - this.warnTimeout * 1000);

            if (this.errorTimeout)
                this.errorHandle = window.setTimeout(() => this.error(), tokenExpiresAt.getTime() - new Date().getTime());
        }
    }

    private info() {

        let msg: IPayloadMessage = {
            text: this.$t('token.expiresAt', [this.user.exp.toLocaleString()]).toString(),
            messageTypeId: PayloadMessageTypes.info
        }

        this.$store.dispatch(StoreTypes.updateStatusBar, msg);
    }

    private warn() {
        let msg: IPayloadMessage = {
            text: this.$t('token.expiresAt', [this.user.exp.toLocaleString()]).toString(),
            messageTypeId: PayloadMessageTypes.warning
        }

        this.$store.dispatch(StoreTypes.updateStatusBar, msg);
    }

    private error() {

        let msg: IPayloadMessage = {
            text: this.$t('token.expired').toString(),
            messageTypeId: PayloadMessageTypes.error
        }

        let logout = this.logout === undefined || this.logout === null ? true : this.logout;

        if (logout)
            this.authenticationService.logout()
                .then(value => this.$store.dispatch(StoreTypes.updateUser, value))
                .then(() => this.$store.dispatch(StoreTypes.updateStatusBar, msg))
                .then(() => this.$router.push({ name: 'login' }));
        else
            this.$store.dispatch(StoreTypes.updateStatusBar, msg)
    }

    $router: IRouterMixinData;

    $store: Store<{}>;
}