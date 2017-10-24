import Vue from 'vue';
import { Route, default as VueRouter } from 'vue-router';

export type IRouterMixinData = VueRouter;

export type IRouteMixinData = Route;

export interface IRouterMixin {
    $route: IRouteMixinData;
    $router: IRouterMixinData;
}

