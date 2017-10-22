import { ICommonState } from '../../store';

export interface IRootState {
    apiCallContent: string;
}

export interface IRootStoreState extends IRootState {
    common?: ICommonState
}