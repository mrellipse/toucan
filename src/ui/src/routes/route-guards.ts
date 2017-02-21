import Vue = require('vue');
import { NavigationGuard, RawLocation, Route, RouteRecord } from 'vue-router';
import { AuthenticationService, IClaimsHelper } from '../services';
import { IUser } from '../model';
import { IRouteMeta } from './route-meta';

function routeCheck(helper: IClaimsHelper, meta: IRouteMeta): boolean {

    let user = AuthenticationService.getUser();

    if (!meta.private && !meta.roles)
        return false;

    if (user.authenticated && !meta.roles)
        return false;

    if (meta.roles && meta.roles.length > 0) {
        return meta.roles.find(o => helper.isInRole(user, o)) === undefined;
    }

    return true;
}

export function RouteGuards(loginPageName: string): NavigationGuard {

    let fn = (to: Route, from: Route, next: (to?: RawLocation | false | ((vm: Vue) => any) | void) => void) => {

        let auth = new AuthenticationService();
        
        if (to.matched.some(r => routeCheck(auth, r.meta))) {

            let sendTo: RawLocation = {
                name: loginPageName,
                query: { returnUrl: to.fullPath }
            };

            next(sendTo);

        } else {
            next();
        }
    };

    return fn;
}

