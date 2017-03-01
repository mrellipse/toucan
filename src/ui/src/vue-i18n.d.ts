declare module "vue-i18n" {

    import _Vue = require("vue");

    /**
     * @module augmentation to ComponentOptions defined by Vuejs
    */
    module "vue/types/options" {

        interface ComponentOptions<V extends _Vue> {
            locales?: any;
        }
    }

    type stringOrNum = string | number;

    interface ICustomFormatter { (resource: string, ...args: any[]): string }
    interface IFormatter { (keypath: string, lang?: string, ...args: any[]): string }
    interface IKeypathChecker { (keypath: string, lang?: string): boolean }
    interface ILocale { (lang: string, success: () => Promise<{}>, failure: () => void): void }
    interface ILocale { (lang: string, locale: {}): void }
    interface IPluralizer { (keypath: string, choice: number, ...args: any[]): string; }

    export type Formatter = IFormatter;
    /**
     * Register or retrieve a locale
    */
    export type Locale = ILocale;
    export type Pluralizer = IPluralizer;
    export type KeypathChecker = IKeypathChecker;

    type GlobalConfig = {
        fallbackLang: string,
        i18nFormatter: ICustomFormatter,
        lang: string,
        missingHandler: (lang: string, key: string, vm: _Vue) => void
        t: IFormatter,
        tc: IPluralizer,
        te: IKeypathChecker
    }

    export interface I18n {
        $lang: string
        $t: IFormatter
        $tc: IPluralizer
        $te: IKeypathChecker
    }

    function plugin(Vue: typeof _Vue, opts: {}): void;

    export default plugin;
}


/**
 * @module augmentation to default Vue instance
*/

declare module "vue/types/vue" {
    import { Locale } from 'vue-i18n';
    interface Vue {
        locale: Locale
    }
}
