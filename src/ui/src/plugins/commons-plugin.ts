import Vue = require('vue');
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import { PluginFunction } from "vue/types/plugin";
import { ICommonState, StoreTypes } from '../store';

interface IPluginOptions {
    store: Store<{ common: ICommonState }>,
}

export interface ICommonOptions {
    exec: <T>(cb: PromiseLike<T>, handleErrors?: boolean) => Promise<T>
}

export function CommonsPlugin<ICommonsPlugin>(vue: typeof Vue, options?: IPluginOptions) {

    // common methods helper
    (<any>vue.prototype).$common = {
        exec: <T>(cb: PromiseLike<T>, handleErrors?: boolean) => {

            var data: T = null;

            let onError = (e: Error) => {
                options.store.dispatch(StoreTypes.loadingState, false).then(() => {
                    options.store.dispatch(StoreTypes.updateStatusBar, e);
                });
            };
            
            handleErrors = handleErrors || true;

            let p = options.store.dispatch(StoreTypes.loadingState, true)
                .then(() => cb)
                .then(value => data = value)
                .then(() => options.store.dispatch(StoreTypes.loadingState, false))
                .then(() => data);

            if (handleErrors)
                p.catch(onError);

            return p;
        }
    };

}

export default CommonsPlugin;