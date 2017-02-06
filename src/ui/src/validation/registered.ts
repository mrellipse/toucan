import { routes } from '../main';
import { IPayload } from '../model';
import { AuthenticationHelper, PayloadMessageTypes } from '../helpers';

export function registered(username: string) {

    let onSuccess = (payload: IPayload<boolean>) => {
        return payload.message.messageTypeId !== PayloadMessageTypes.failure;
    }

    let auth = new AuthenticationHelper();

    return (<any>auth.validateUsername(username))
        .then(onSuccess);
};

export default registered;