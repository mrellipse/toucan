import { default as Axios } from 'axios';
import { LocaleMessageObject } from 'vue-i18n';
import { ApiService } from './api-service';
import { KeyValue } from '../model';
import { GlobalConfig } from '../common';

const BASE_URL = GlobalConfig.uri.services + 'culture/';

export interface ICultureData {
    cultureName: string
    resources?: string | {}
}


export function MapLocaleMessages(data: ICultureData): LocaleMessageObject {

    if (typeof data.resources == 'string') {
        return JSON.parse(data.resources);
    }
}

export class CultureService extends ApiService {

    constructor() {
        super();
    }

    static mapLocaleMessages(data: ICultureData): LocaleMessageObject {
        return MapLocaleMessages(data);
    }

    resolveCulture(id: string) {
        return this.exec<ICultureData>(Axios.get(BASE_URL + 'ResolveCulture', { params: { id } }))
            .then((value) => this.processPayload(value));
    }

    supportedCultures() {
        return this.exec<KeyValue[]>(Axios.get(BASE_URL + 'SupportedCultures', null))
            .then((value) => this.processPayload(value));
    }

    supportedTimeZones() {
        return this.exec<KeyValue[]>(Axios.get(BASE_URL + 'SupportedTimeZones', null))
            .then((value) => this.processPayload(value));
    }

    updateCulture(id: string) {
        return this.exec<{}>(Axios.get(BASE_URL + 'UpdateCulture', { params: { id } }))
            .then((value) => this.processPayload(value));
    }
}