import { default as Axios, AxiosResponse } from 'axios';
import { IPayload } from '../model';
import { GlobalConfig, PayloadMessageTypes } from '../common';

const AUTH_URL = GlobalConfig.uri.auth;
const VALIDATE_USER_URL = AUTH_URL + 'validateuser';

export function registered(userName: string) {

    let config = {
        params: { userName: userName }
    };

    let onSuccess = (res: AxiosResponse) => {
        let payload: IPayload<boolean> = res.data;
        return payload.message.messageTypeId !== PayloadMessageTypes.failure;
    }

    return Axios.get(VALIDATE_USER_URL, config)
        .then(onSuccess);

};