
import * as Vue from 'vue';
import Component from 'vue-class-component';
import { Store } from 'vuex';
import { State } from 'vuex-class';
import { PluginFunction } from "vue/types/plugin";
import { StoreTypes } from '../store';
import { IPayload, IPayloadMessage, IStatusBarData, PayloadMapper, PayloadMessageTypes } from '../model';

export interface IStoreService {
    exec: <T>(cb: Promise<T>) => Promise<T>
}

interface IProcessPayloadOptions<T> {
    timeout?: number,
    uri?: string
}

export abstract class StoreService implements IStoreService {

    constructor(protected store: Store<{}>) {

    }

    handleFulfilled<T>(value: T) {

        return this.store.dispatch(StoreTypes.loadingState, false)
            .then(() => new PayloadMapper().fromObject(value));
    }

    handleRejection<T>(reason: any) {

        return this.store.dispatch(StoreTypes.loadingState, false)
            .then(() => this.store.dispatch(StoreTypes.updateStatusBar, reason))
            .then(() => new PayloadMapper().fromObject(reason));
    }

    processPayload<T>(payload: IPayload<T>, options?: IProcessPayloadOptions<T>): Promise<T> {

        let message = payload.message;
        options = options || {};

        if (message.messageTypeId === PayloadMessageTypes.error) {

            options.timeout = options.timeout || 1500;

            let statusBarData: IStatusBarData = {
                messageTypeId: message.messageTypeId,
                text: message.text,
                timeout: options.timeout,
                title: message.title,
                uri: options.uri
            };

            return this.store.dispatch(StoreTypes.updateStatusBar, statusBarData)
                .then(() => Promise.resolve(null));
                
        } else {
            return Promise.resolve(payload.data);
        }
    }

    exec<T>(cb: Promise<{}>): Promise<IPayload<T>> {

        let onFulfilled = (value) => this.handleFulfilled(value);
        let onRejection = (value) => this.handleRejection(value)

        return this.store.dispatch(StoreTypes.loadingState, true)
            .then(() => cb)
            .then(onFulfilled, onRejection);
    }
}