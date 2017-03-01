import VueRouter = require('vue-router');
import { PageForbidden, Login, PageNotFound, Search, Verify } from '../../components';
import { AreaLayout } from '../layout/layout';
import { Home } from '../home/home';
import { Profile } from '../profile/profile';
import { Signup } from '../signup/signup';
import { RouteNames } from './route-names';

export const RouteConfig: VueRouter.RouteConfig[] = [
    {
        component: AreaLayout,
        path: '/',
        children: [
            {
                component: Home,
                name: RouteNames.home,
                path: ''
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
                component: Search,
                name: RouteNames.search,
                path: '/search/:searchText'
            },
            {
                component: Search,
                name: RouteNames.search + 'default',
                path: '/search'
            },
            {
                component: Verify,
                name: RouteNames.login.verify,
                path: '/verification'
            }
        ]
    },
    {
        component: Login,
        name: RouteNames.login.home,
        path: '/login'
    },
    {
        component: Signup,
        name: RouteNames.signup,
        path: '/signup',
    },
    {
        component: PageForbidden,
        name: RouteNames.forbidden,
        path: '/forbidden'
    },
    {
        component: PageNotFound,
        path: '*'
    }
];