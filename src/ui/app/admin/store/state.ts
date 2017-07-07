import { ICommonState } from '../../store';

export interface IAdminState {

}

export interface IAdminStoreState extends IAdminState {
    common?: ICommonState
}