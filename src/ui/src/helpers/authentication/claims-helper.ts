
import { IUser } from '../../model';

export interface IClaimsHelper {
    isInRole(user:IUser, role: string): boolean;
}