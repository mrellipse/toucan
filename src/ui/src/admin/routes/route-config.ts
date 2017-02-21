import VueRouter = require('vue-router');
import { Login, PageNotFound, Search } from '../../components';
import { AreaLayout } from '../layout/layout';
import { AreaDashboard } from '../dashboard/dashboard';
import { SiteSettings } from '../settings/settings';
import { SiteReports } from '../reports/reports';
import { UserRoles } from '../../model';
import { RouteNames } from './route-names';

export const RouteConfig: VueRouter.RouteConfig[] = [
    {
        component: Login,
        name: RouteNames.login,
        path: '/login'
    },
    {
        component: AreaLayout,
        name: RouteNames.home,
        path: '/',
        meta: {
            roles: [UserRoles.Admin]
        },
        children: [
            {
                component: AreaDashboard,
                name: RouteNames.dashboard,
                path: ''
            },
            {
                component: SiteReports,
                name: RouteNames.siteReports,
                path: '/reports'
            },
            {
                component: SiteSettings,
                name: RouteNames.siteSettings,
                path: '/site'
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
            }
        ]
    },
    {
        component: PageNotFound,
        path: '*'
    }
];