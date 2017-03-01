import { Store, ActionContext, ActionTree } from 'vuex';
import { PayloadMessageTypes, TokenHelper } from '../common';
import { IPayloadMessage, IStatusBarData, IUser } from '../model';
import { ICommonState } from './state';
import { StoreTypes } from './types';

function isStatusBarData(object: any): object is IStatusBarData {
    return object === undefined ? null : 'uri' in object;
}

function isPayloadMessage(object: any): object is IPayloadMessage {
    return object === undefined ? null : 'messageTypeId' in object;
}

export const Actions: ActionTree<ICommonState, any> = {

    loading: (injectee: ActionContext<ICommonState, any>, loading: boolean) => {

        injectee.commit(StoreTypes.loadingState, loading);
    },

    updateUser: (injectee: ActionContext<ICommonState, any>, userData: string | IUser) => {

        let payload: IUser = null;

        if (typeof userData === 'string')
            payload = TokenHelper.parseUserToken(userData);
        else
            payload = userData

        injectee.commit(StoreTypes.updateUser, payload);
    },

    updateStatusBar: (injectee: ActionContext<ICommonState, any>, data: Error | IStatusBarData | IPayloadMessage) => {

        let payload: IStatusBarData = null;

        if (data === null) {
            payload = {
                messageTypeId: null,
                text: null,
                title: null,
                uri: null
            };
        }
        else if (data instanceof Error) {

            payload = {
                messageTypeId: PayloadMessageTypes.error,
                title: data.name,
                text: data.message,
                uri: null
            };

        } else if (isStatusBarData(data)) {

            payload = data;

        } else if (isPayloadMessage(data)) {

            payload = {
                messageTypeId: data.messageTypeId,
                title: data.title,
                text: data.text,
                uri: null
            };

        }

        if (payload)
            injectee.commit(StoreTypes.updateStatusBar, payload);

    }
};

export default Actions;