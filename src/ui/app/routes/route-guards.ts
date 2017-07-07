import * as Vue from 'vue';
import { NavigationGuard, RawLocation, Route, RouteRecord } from 'vue-router';
import { AuthenticationService, IClaimsHelper } from '../services';
import { TokenHelper } from '../common';
import { IUser } from '../model';
import { IRouteMeta } from './route-meta';

interface IRouteGuardOptions {
    resolveUser(): IUser;
    forbiddenRouteName: string;
    loginRouteName: string;
    verifyRouteName: string;
    store: any;
}

function routeCheck(user: IUser, helper: IClaimsHelper, meta: IRouteMeta): boolean {

    if (!meta.private && !meta.roles)
        return false;

    if (user.authenticated && !meta.roles)
        return false;

    if (meta.roles && meta.roles.length > 0) {
        return meta.roles.find(o => helper.isInRole(user, o)) === undefined;
    }

    return true;
}

function verifyCheck(user: IUser, meta: IRouteMeta): boolean {

    if (user.authenticated && (meta.private || meta.roles))
        return !user.verified;
    else
        return false;
}

export function RouteGuards(options: IRouteGuardOptions): NavigationGuard {

    let fn = (to: Route, from: Route, next: (to?: RawLocation | false | ((vm: Vue) => any) | void) => void) => {

        let claimsHelper: IClaimsHelper = new AuthenticationService(options.store);

        let user = options.resolveUser() || TokenHelper.parseUserToken(TokenHelper.getAccessToken());

        if (to.matched.some(r => routeCheck(user, claimsHelper, r.meta))) {

            let sendTo: RawLocation = null;

            if (user.authenticated && to.meta.roles) {
                sendTo = {
                    name: options.forbiddenRouteName
                }
            } else {
                sendTo = {
                    name: options.loginRouteName,
                    query: { returnUrl: to.fullPath }
                }
            }

            next(sendTo);

        } else if (to.name !== options.verifyRouteName && to.matched.some(r => verifyCheck(user, r.meta))) {

            let sendTo: RawLocation = {
                name: options.verifyRouteName,
                query: { returnUrl: to.fullPath }
            };

            next(sendTo);
        }
        else {
            next();
        }
    };

    return fn;
}

