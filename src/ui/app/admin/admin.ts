import * as Vue from 'vue';
import { default as VueRouter } from 'vue-router';
import { default as Vuelidate } from 'vuelidate';
import VueI18n from 'vue-i18n';
import { RouteGuards, RouteNames, RouterOptions } from './routes';
import { GlobalConfig, TokenHelper, UseAxios } from '../common';
import { i18n } from '../locales';
import { Loader, StatusBar } from '../components';
import * as Plugins from '../plugins';
import { AreaNavigation } from './navigation/navigation';
import { AdminStoreTypes } from './store';
import { AreaFooter } from './footer/footer';
import './admin.scss';

import { Store } from './store/store';  // global state management

Vue.use(Vuelidate); // validation
Vue.use(VueRouter); // router

const router = new VueRouter(RouterOptions);
let routeOptions = {
    resolveUser: () => Store.state.common.user,
    forbiddenRouteName: RouteNames.forbidden,
    loginRouteName: RouteNames.login,
    verifyRouteName: RouteNames.verify,
    store: Store
};

router.beforeEach(RouteGuards(routeOptions));

Vue.use(Plugins.CommonsPlugin, {
    store: <never>Store,
});

Vue.use(Plugins.UserOptionsPlugin, {
    key: GlobalConfig.uopt,
    default: { locale: 'en' },
    store: <never>Store,
    watchLocaleChanges: true
});

UseAxios(router);

Vue.component('status-bar', StatusBar);

export const app = new Vue({

    components: {
        AreaFooter,
        Loader,
        Navigation: AreaNavigation
    },

    i18n,

    router,

    store: Store,

    created() {

        // check if location hash has state/nonce value ...
        let resumeExternalLogin = () => {

            if (location.hash && location.hash.indexOf("state") != -1) {

                let hash = location.hash.substring(1);

                if (hash.indexOf("access_token") != -1 || hash.indexOf("error") != -1) {
                    router.push({
                        name: RouteNames.login,
                        query: { hash: hash }
                    });
                }
            }
        }

        let token = TokenHelper.getAccessToken();

        Store.dispatch(AdminStoreTypes.common.updateUser, token)
            .then(() => Store.dispatch(AdminStoreTypes.common.loadingState, false))
            .then(() => resumeExternalLogin());
    },

    computed: {

        isLoading: () => Store.state.common.isLoading
    }

}).$mount('#app');
