import { IStatusBarData, IUser } from '../model';

export interface ICommonState {
    isLoading: boolean;
    user: IUser;
    statusBar: IStatusBarData;
}