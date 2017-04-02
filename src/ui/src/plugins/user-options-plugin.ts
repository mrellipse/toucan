import Vue = require('vue');
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import { PluginFunction } from "vue/types/plugin";
import { IOptionsService, OptionsService } from './user-options-service';
import { ICommonState, StoreTypes } from '../store';
import { IUserOptions } from '../model';

interface IPluginOptions {
    key: string;
    default: IUserOptions;
    store: Store<{ common: ICommonState }>,
    watchLocaleChanges?: boolean
}

export function UserOptionsPlugin<IUserOptionsPlugin>(vue: typeof Vue, options?: IPluginOptions) {

    let svc = new OptionsService<IUserOptions>(options.key, options.default);
    let userOptions = svc.ensure();
    
    // set a watcher on the store for locale changes
    if (options.watchLocaleChanges || true) {
        options.store.watch((state) => state.common.userOptions, (value, oldValue) => {
            if (value.locale && value.locale !== oldValue.locale) {
                let userOptions = Object.assign({}, svc.options);
                (<any>vue).config.lang = userOptions.locale = value.locale;
                svc.options = userOptions;
            }
        }, null);
    }

    // load default user settings into Vuex Store ...
    options.store.dispatch(StoreTypes.updateUserOptions, userOptions);
}

export default UserOptionsPlugin;