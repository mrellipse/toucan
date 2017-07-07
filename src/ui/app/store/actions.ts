import { Store, ActionContext, ActionTree } from 'vuex';
import { PayloadMessageTypes, TokenHelper } from '../common';
import { IPayloadMessage, IStatusBarData, IUser, IUserOptions } from '../model';
import { ICommonState } from './state';
import { StoreTypes } from './types';

function isStatusBarData(object: any): object is IStatusBarData {
    return object === undefined ? null : 'uri' in object || 'timeout' in object;
}

function isPayloadMessage(object: any): object is IPayloadMessage {
    return object === undefined ? null : 'messageTypeId' in object;
}

export const Actions: ActionTree<ICommonState, any> = {

    loading: (injectee: ActionContext<ICommonState, any>, loading: boolean) => {

        injectee.commit(StoreTypes.loadingState, loading);
    },

    updateLocale: (injectee: ActionContext<ICommonState, any>, lang: string) => {

        injectee.commit(StoreTypes.updateLocale, lang);
    },

    updateUser: (injectee: ActionContext<ICommonState, any>, userData: string | IUser) => {

        let payload: IUser = null;

        if (typeof userData === 'string')
            payload = TokenHelper.parseUserToken(userData);
        else
            payload = userData

        injectee.commit(StoreTypes.updateUser, payload);
    },

    updateUserOptions: (injectee: ActionContext<ICommonState, any>, userOptions: IUserOptions) => {

        injectee.commit(StoreTypes.updateUserOptions, userOptions);
    },

    updateStatusBar: (injectee: ActionContext<ICommonState, any>, data: Error | IStatusBarData | IPayloadMessage) => {

        let payload: IStatusBarData = null;

        if (data === null) {
            payload = {
                messageTypeId: null,
                text: null,
                timeout: null,
                title: null,
                uri: null
            };
        }
        else if (data instanceof Error) {

            payload = {
                messageTypeId: PayloadMessageTypes.error,
                text: data.message,
                timeout: null,
                title: data.name,
                uri: null
            };

        } else if (isStatusBarData(data)) {

            payload = data;

        } else if (isPayloadMessage(data)) {

            payload = {
                messageTypeId: data.messageTypeId,
                text: data.text,
                timeout: null,
                title: data.title,
                uri: null
            };

        }

        if (payload)
            injectee.commit(StoreTypes.updateStatusBar, payload);
    }
};