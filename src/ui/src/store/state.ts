import { IUser } from '../model';

export interface ICommonState {
    isLoading: boolean;
    user: IUser;
}