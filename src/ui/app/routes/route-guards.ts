import Vue from "vue";
import { NavigationGuard, RawLocation, Route } from "vue-router";
import { AuthenticationService, IClaimsHelper } from "../services";
import { isNil } from "lodash/fp";
import { TokenHelper } from "../common";
import { IUser } from "../model";
import { IRouteMeta } from "./route-meta";

interface IRouteGuardOptions {
  resolveUser(): IUser;
  forbiddenRouteName: string;
  loginRouteName: string;
  verifyRouteName: string;
  store: any;
}

function routeCheck(
  user: IUser,
  helper: IClaimsHelper,
  meta: IRouteMeta
): boolean {
  let hasClaims = !isNil(meta.claims);
  let matchAny = isNil(meta.any) ? true : meta.any;

  if ((hasClaims || meta.private) && !user.authenticated) return true;

  if (hasClaims) {
    if (Array.isArray(meta.claims)) {
      if (matchAny) {
        return !helper.satisfiesAny(user, meta.claims);
      } else {
        return !helper.satisfies(user, meta.claims);
      }
    } else {
      return true;
    }
  } else {
    return false;
  }
}

function verifyCheck(user: IUser, meta: IRouteMeta): boolean {
  if (user.authenticated && (meta.private || meta.claims))
    return !user.verified;
  else return false;
}

export function RouteGuards(options: IRouteGuardOptions): NavigationGuard {
  let fn = (
    to: Route,
    from: Route,
    next: (to?: RawLocation | false | ((vm: Vue) => any) | void) => void
  ) => {
    let claimsHelper: IClaimsHelper = new AuthenticationService(options.store);

    let user =
      options.resolveUser() ||
      TokenHelper.parseUserToken(TokenHelper.getAccessToken());

    if (to.matched.some(r => routeCheck(user, claimsHelper, r.meta))) {
      let sendTo: RawLocation = null;
      
      if (user.authenticated && to.meta.claims) {
        sendTo = {
          name: options.forbiddenRouteName
        };
      } else {
        sendTo = {
          name: options.loginRouteName,
          query: { returnUrl: to.fullPath }
        };
      }

      next(sendTo);
    } else if (
      to.name !== options.verifyRouteName &&
      to.matched.some(r => verifyCheck(user, r.meta))
    ) {
      let sendTo: RawLocation = {
        name: options.verifyRouteName,
        query: { returnUrl: to.fullPath }
      };

      next(sendTo);
    } else {
      next();
    }
  };

  return fn;
}
