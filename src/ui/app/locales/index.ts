import Vue from 'vue';
import VueI18n from 'vue-i18n';
import { CookieHelper } from '../common';
import { KeyValue } from '../model';
import { CultureService, ICultureData } from '../services'

Vue.use(VueI18n);

export const SupportedLocales: string[] = [];
export const SupportedTimeZones: KeyValue[] = [];

export function InitI18n() {

    let loading = false;
    let locale = CookieHelper.getCultureName() || "en";
    let svc = new CultureService();

    return updateSupportedLocales()
        .then(updateSupportedTimeZones)
        .then(() => svc.resolveCulture(locale))
        .then(CultureService.mapLocaleMessages)
        .then(onFulfilled);

    function onFulfilled(message: VueI18n.LocaleMessage) {

        let messages = {};
        messages[locale] = message;

        return new VueI18n({
            fallbackLocale: locale,
            locale: locale,
            silentTranslationWarn: false,
            messages: messages
        });
    }

    function updateSupportedLocales() {

        let onFulfilled = (data: KeyValue[]) => data.forEach(o => SupportedLocales.push(o.key));

        if (SupportedLocales.length == 0)
            return svc.supportedCultures().then(onFulfilled);
        else
            return Promise.resolve();
    }

    function updateSupportedTimeZones() {

        let onFulfilled = (data: KeyValue[]) => data.forEach(o => SupportedTimeZones.push(o));

        if (SupportedTimeZones.length == 0)
            return svc.supportedTimeZones().then(onFulfilled);
        else
            return Promise.resolve();
    }
};