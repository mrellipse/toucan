import { Store as VuexStore } from 'vuex';
import { IRootStoreState } from './state';
import { RootStoreTypes } from './types';

export { IRootStoreState } from './state';

export { RootStoreTypes } from './types';

export type RootStore = VuexStore<IRootStoreState>;