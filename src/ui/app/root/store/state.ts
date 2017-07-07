import { ICommonState } from '../../store';

export interface IRootState {
    secureContent: string;
}

export interface IRootStoreState extends IRootState {
    common?: ICommonState
}