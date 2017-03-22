import { IStatusBarData, IUser, IUserOptions } from '../model';

export interface ICommonState {
    isLoading: boolean;
    user: IUser;
    userOptions: IUserOptions;
    statusBar: IStatusBarData;
}