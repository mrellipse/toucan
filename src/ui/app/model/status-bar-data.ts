import { IPayloadMessage } from './payload';

export interface IStatusBarData extends IPayloadMessage {
    uri?: string;
    timeout?: number;
}