import { Store as VuexStore } from 'vuex';
import { IAdminStoreState } from './state';
import { RootStoreTypes } from './types';

export { IAdminStoreState } from './state';

export { RootStoreTypes } from './types';

export type RootStore = VuexStore<IAdminStoreState>;