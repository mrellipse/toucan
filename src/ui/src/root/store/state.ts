import { ICommonState } from '../../store';

export interface IRootState {

}

export interface IRootStoreState extends IRootState {
    common?: ICommonState
}