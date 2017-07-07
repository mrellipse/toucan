import * as Vue from 'vue';
import { en } from './en';
import { fr } from './fr';
import VueI18n from 'vue-i18n';

Vue.use(VueI18n);

export const SupportedLocales = ['en', 'fr'];

var locale = resolveLocale(window.navigator.language) || "en";

export const i18n = new VueI18n({
    locale: locale,
    fallbackLocale: 'en',
    silentTranslationWarn: false,
    messages: {
        'en': en,
        'fr': fr
    }
});

function resolveLocale(lang){
    
    if(SupportedLocales.indexOf(lang) != -1)
        return lang;
    
    let locale = SupportedLocales.find((o) => {
        return lang.toLowerCase().startsWith(o.toLowerCase());
    });

    return locale;
}
