import { default as Axios } from 'axios';
import { ApiService } from './api-service';
import { GlobalConfig } from '../common';
import { IUser } from '../model';

const BASE_URL = GlobalConfig.uri.services + 'profile/';

export interface IUserCultureData {
    cultureName: string
    resources?: { key: string, value: string }[]
    timeZoneId: string
}

interface IUpdateUserCultureOptions {
    userId?: number
    cultureName: string
    timeZoneId?: string
}

export class ProfileService extends ApiService {

    constructor() {
        super();
    }

    updateUserCulture(options: IUpdateUserCultureOptions) {

        return this.exec<IUserCultureData>(Axios.put(BASE_URL + 'UpdateUserCulture', options))
            .then((value) => this.processPayload(value));
    }
}