
import { IUser } from '../../model';

export interface IClaimsHelper {
    satisfies(user: IUser, claims: string[]): boolean;
    satisfiesAny(user: IUser, claims: string[]): boolean;
}