import { auth, routes } from '../main';
import { IPayload } from '../model';
import { PayloadMessageTypes } from '../helpers/message';

export function registered(username: string) {

    let onSuccess = (payload: IPayload<boolean>) => {
        return payload.message.messageTypeId !== PayloadMessageTypes.failure;
    }

    return (<any>auth.validateUsername(username))
        .then(onSuccess);
};

export default registered;