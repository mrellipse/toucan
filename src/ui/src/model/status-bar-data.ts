import { IPayloadMessage } from './payload-message';

export interface IStatusBarData extends IPayloadMessage {
    uri?: string;
}