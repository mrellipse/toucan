import { Store as VuexStore } from 'vuex';
import { IAdminStoreState } from './state';
import { AdminStoreTypes } from './types';

export { IAdminStoreState } from './state';

export { AdminStoreTypes } from './types';

export type AdminStore = VuexStore<IAdminStoreState>;