import VueRouter = require('vue-router');
import { Home, Login, PageNotFound, Profile, Search, Signup } from '../components';
import { RouteNames } from './route-names';

export const RouteConfig: VueRouter.RouteConfig[] = [
    {
        component: Home,
        name: RouteNames.home,
        path: '/'
    },
    {
        component: Login,
        name: RouteNames.login,
        path: '/login'
    },
    {
        component: Search,
        name: RouteNames.searchText,
        path: '/search/:searchText'
    },
    {
        component: Search,
        name: RouteNames.search,
        path: '/search'
    },
    {
        component: Signup,
        name: RouteNames.signup,
        path: '/signup',
    },
    {
        component: Profile,
        name: RouteNames.profile,
        path: '/profile',
        meta: {
            private: true
        }
    },
    {
        component: Profile,
        name: RouteNames.admin,
        path: '/admin',
        meta: {
            roles: ['Admin']
        }
    },
    {
        component: PageNotFound,
        path: '*'
    }
];