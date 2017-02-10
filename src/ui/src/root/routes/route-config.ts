import VueRouter = require('vue-router');
import { Login, PageNotFound, Search } from '../../components';
import { AreaLayout } from '../layout/layout';
import { Home } from '../home/home';
import { Profile } from '../profile/profile';
import { Signup } from '../signup/signup';
import { RouteNames } from './route-names';

export const RouteConfig: VueRouter.RouteConfig[] = [
    {
        component: AreaLayout,
        name: RouteNames.root,
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
                name: RouteNames.searchText,
                path: '/search/:searchText'
            },
            {
                component: Search,
                name: RouteNames.search,
                path: '/search'
            }
        ]
    },
    {
        component: Login,
        name: RouteNames.login,
        path: '/login'
    },
    {
        component: Signup,
        name: RouteNames.signup,
        path: '/signup',
    },
    {
        component: PageNotFound,
        path: '*'
    }
];