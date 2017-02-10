import Vue = require('vue');
import { NavigationGuard, RawLocation, Route, RouteRecord } from 'vue-router';
import { AuthenticationHelper, IClaimsHelper } from '../helpers';
import { IUser } from '../model';
import { IRouteMeta } from './route-meta';

function routeCheck(user: IUser, helper: IClaimsHelper, meta: IRouteMeta): boolean {

    if (!meta.private && !meta.roles)
        return false;

    if (user.authenticated)
        return false;

    if (meta.roles && meta.roles.length > 0) {
        return meta.roles.find(o => helper.isInRole(o)) === undefined;
    }

    return true;
}

export function RouteGuards(loginPageName: string): NavigationGuard {

    let fn = (to: Route, from: Route, next: (to?: RawLocation | false | ((vm: Vue) => any) | void) => void) => {

        let auth = new AuthenticationHelper();

        if (to.matched.some(r => routeCheck(auth.user, auth, r.meta))) {

            let sendTo: RawLocation = {
                name: loginPageName,
                query: { redirect: to.fullPath }
            };

            next(sendTo);

        } else {
            next();
        }
    };

    return fn;
}

