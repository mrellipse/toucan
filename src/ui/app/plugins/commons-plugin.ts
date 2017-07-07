import * as Vue from 'vue';
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import { PluginFunction } from "vue/types/plugin";
import { ICommonState, StoreTypes } from '../store';

interface IPluginOptions {
    store: Store<{ common: ICommonState }>,
}

export interface ICommonOptions {
    extendMe: () => boolean;
}

export function CommonsPlugin<ICommonsPlugin>(vue: typeof Vue, options?: IPluginOptions) {

    // common methods helper
    (<any>vue.prototype).$common = {
        extendMe: () => true
    };

}