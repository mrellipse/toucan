import VueRouter = require('vue-router');
import { PageForbidden, Login, PageNotFound, Search, Verify } from '../../components';
import { AreaLayout } from '../layout/layout';
import { AreaDashboard } from '../dashboard/dashboard';
import { ManageUser } from '../users/user';
import { ManageUserList } from '../users/user-list';
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
                component: AreaDashboard,
                name: RouteNames.home,
                path: ''
            }, {
                component: ManageUser,
                name: RouteNames.manageUser,
                path: 'users/:id',
                props: true
            },
            {
                component: ManageUserList,
                name: RouteNames.manageUsers,
                path: 'users',
                props: function (route: VueRouter.Route) {

                    let page = Number.parseInt(route.query['page']);
                    let pageSize = Number.parseInt(route.query['pageSize']);

                    return {
                        page: isNaN(page) ? 1 : page,
                        pageSize: isNaN(pageSize) ? 5 : pageSize
                    }
                }
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
            },
            {
                component: Verify,
                name: RouteNames.verify,
                path: '/verification'
            }
        ]
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