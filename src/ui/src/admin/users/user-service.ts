import { default as Axios, AxiosRequestConfig, AxiosResponse } from 'axios';
import { IKeyValueList, IPayload, ISearchResult, IUser } from '../../model';
import { GlobalConfig, PayloadMessageTypes } from '../../common';

// URL and endpoint constants
const BASE_URL = GlobalConfig.uri.services + 'manage/user/';

const SEARCH_URL = BASE_URL + 'search';
const GET_USER_URL = BASE_URL + 'GetUser';
const UPDATE_STATUS_URL = BASE_URL + 'UpdateUserStatus';
const UPDATE_USER_URL = BASE_URL + 'UpdateUser';

interface IGetUserPayload {
    user: IUser,
    availableRoles: IKeyValueList<string,string>
}

export class ManageUserService {

    getUser(id: string | number) {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IGetUserPayload> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                return payload.data;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.get(GET_USER_URL, { params: { id } })
            .then(onSuccess);
    }

    search(page: number, pageSize: number) {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<ISearchResult<IUser>> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                return payload.data;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.get(SEARCH_URL, { params: { page, pageSize } })
            .then(onSuccess);
    }

    updateUser(options: IUser) {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IUser> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                return payload.data;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.put(UPDATE_USER_URL, options)
            .then(onSuccess);
    }

    updateUserStatus(options: { username: string, enabled: boolean, verified: boolean }) {

        let onSuccess = (res: AxiosResponse) => {

            let payload: IPayload<IUser> = res.data;

            if (payload.message.messageTypeId === PayloadMessageTypes.success) {
                return payload.data;
            } else {
                throw new Error(payload.message.text);
            }
        }

        return Axios.put(UPDATE_STATUS_URL, options)
            .then(onSuccess);
    }
}

export default ManageUserService;
