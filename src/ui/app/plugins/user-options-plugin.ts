import Vue from 'vue';
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import { default as VueI18n } from 'vue-i18n';
import { PluginFunction } from "vue/types/plugin";
import { CookieHelper } from "../common";
import { IUser } from '../model';
import { MapLocaleMessages, ProfileService } from '../services';
import { ICommonState, StoreTypes } from '../store';

interface IPluginOptions {
    i18n: VueI18n;
    options?: IUser,
    store: Store<{ common: ICommonState }>,
    watchLocaleChanges?: boolean
}

export function UserOptionsPlugin<IUserPlugin>(vue: typeof Vue, options?: IPluginOptions) {

    let cache: IUser = null;
    let profileService: ProfileService = null;

    // watch the entire user object, and refresh
    var initHandle = options.store.watch((state) => state.common.user, (value, oldValue) => {
        if (value && cache == null) {
            cache = Object.assign({}, value);
            initHandle();
        }
    }, null);

    // set a watcher on the store for locale changes
    if (options.watchLocaleChanges || true) {
        options.store.watch((state) => state.common.user, (value, oldValue) => {

            let cultureName = value.cultureName;
            let timeZoneId = value.timeZoneId;

            let cultureChanged = cultureName && cultureName !== cache.cultureName;
            let timeZoneChanged = timeZoneId && timeZoneId !== cache.timeZoneId;

            if (cultureChanged || timeZoneChanged) {

                let onFulfilled = (data) => {
                    cache.timeZoneId = data.timeZoneId;
                    
                    if (data.resources) {
                        options.i18n.setLocaleMessage(data.cultureName, MapLocaleMessages(data));
                        options.i18n.locale = cache.cultureName = data.cultureName;
                    }
                };

                let data = {
                    cultureName: cultureName,
                    userId: value.userId || 0,
                    timeZoneId: value.timeZoneId
                };

                profileService = profileService || new ProfileService();

                profileService.updateUserCulture(data)
                    .then(onFulfilled);
            }

        }, null);
    }
}