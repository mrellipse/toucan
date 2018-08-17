import { default as Axios } from 'axios';
import { ISearchResult, IUser, KeyValue } from '../../model';
import { GlobalConfig } from '../../common';
import { Store, StoreService } from '../../store';

// URL and endpoint constants
const BASE_URL = GlobalConfig.uri.services + 'manage/user/';

const SEARCH_URL = BASE_URL + 'search';
const GET_USER_URL = BASE_URL + 'GetUser';
const UPDATE_STATUS_URL = BASE_URL + 'UpdateUserStatus';
const UPDATE_USER_URL = BASE_URL + 'UpdateUser';

export interface IGetUserPayload {
    user: IUser,
    availableRoles: KeyValue[]
}

export class ManageUserService extends StoreService {

    constructor(store: Store<{}>) {
        super(store);
    }

    getUser(id: string | number) {

        return this.exec<IGetUserPayload>(Axios.get(GET_USER_URL, { params: { id } }))
            .then((value) => this.processPayload(value));
    }

    search(page: number, pageSize: number) {

        return this.exec<ISearchResult<IUser>>(Axios.get(SEARCH_URL, { params: { page, pageSize } }))
            .then((value) => this.processPayload(value));
    }

    updateUser(options: IUser) {

        return this.exec<IUser>(Axios.put(UPDATE_USER_URL, options))
            .then((value) => this.processPayload(value));
    }

    updateUserStatus(options: { username: string, enabled: boolean}) {

        return this.exec<IUser>(Axios.put(UPDATE_STATUS_URL, options))
            .then((value) => this.processPayload(value));
    }
}
