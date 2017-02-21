import { IPayload } from '../model';
import { PayloadMessageTypes } from '../common';
import {AuthenticationService } from '../services';

export function registered(username: string) {

    let onSuccess = (payload: IPayload<boolean>) => {
        return payload.message.messageTypeId !== PayloadMessageTypes.failure;
    }

    let auth = new AuthenticationService();

    return (<any>auth.validateUser(username))
        .then(onSuccess);
};

export default registered;