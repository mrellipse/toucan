import { IPayloadMessage } from './payload-message';

export interface IPayload<T>{

    data: T;

    message: IPayloadMessage;
}

export default IPayload;