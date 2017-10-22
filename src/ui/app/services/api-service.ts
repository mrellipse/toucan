
import { IPayload, PayloadMapper, PayloadMessageTypes } from '../model';

export interface IApiService {
    exec: <T>(cb: Promise<{}>) => Promise<IPayload<T>>
}

interface IProcessPayloadOptions<T> {
    messageTypeIds?: string[]
}

export abstract class ApiService implements IApiService {

    constructor() {

    }

    processPayload<T>(payload: IPayload<T>, messageTypeIds?: string[]): Promise<T> {

        let message = payload.message;

        messageTypeIds = messageTypeIds || [PayloadMessageTypes.error, PayloadMessageTypes.failure];

        let messageTypeId = messageTypeIds.find(o => o === message.messageTypeId);

        if (messageTypeId)
            return Promise.reject(payload.message);
        else
            return Promise.resolve(payload.data);

    }

    exec<T>(cb: Promise<{}>): Promise<IPayload<T>> {

        let onFulfilled = (value) => new PayloadMapper().fromObject<T>(value);
        let onRejection = (reason) => new PayloadMapper().fromObject<T>(reason);

        return cb.then(onFulfilled, onRejection);
    }

}