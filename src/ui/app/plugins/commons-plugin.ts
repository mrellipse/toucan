import Vue from "vue";
import { Store } from "vuex";
import { ICommonState, StoreTypes } from "../store";
import { KeyValue } from "../model/key-value";

interface IPluginOptions {
  store: Store<{ common: ICommonState }>;
}

export interface IVueSelectOption {
  id: any;
  label: string;
}

export interface ICommonOptions {
  extendMe: () => boolean;
  mapArrayToOptions: (values: Array<any>) => IVueSelectOption[];
  mapKeyValuesToOptions: (values: KeyValue[]) => IVueSelectOption[];
  mapKeysToOptions: (values: object) => IVueSelectOption[];
}

export function CommonsPlugin<ICommonsPlugin>(
  vue: typeof Vue,
  options?: IPluginOptions
) {
  // common methods helper
  (<any>vue.prototype).$common = {
    extendMe: () => true,
    mapArrayToOptions: (values: Array<any>) => {
      if (values == null || !Array.isArray(values) || values.length == 0)
        return [];

      return values.filter(o => o !== undefined && o !== null).map(o => {
        return { id: o, label: o.toString() };
      });
    },
    mapKeyValuesToOptions: (values: KeyValue[]) => {
      if (values == null || !Array.isArray(values) || values.length == 0)
        return [];

      return values.map(o => {
        return { id: o.key, label: o.value.toString() };
      });
    },
    mapKeysToOptions: (values: object) => {
      if (values == null || values == undefined) return [];

      return Object.keys(values).map(o => {
        return { id: o, label: values[o].toString() };
      });
    }
  };
}
