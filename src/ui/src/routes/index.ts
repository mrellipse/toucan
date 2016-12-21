import VueRouter = require('vue-router');
import { Home, PageNotFound, Post, Posts } from '../components';

const routes: VueRouter.RouteConfig[] = [
  {
    path: '/',
    component: Home
  },
    {
    path: '/posts',
    component: Posts
  },
  {
    path: '/post/:id',
    name: 'post',
    component: Post
  },
  {
    path: '*',
    component: PageNotFound
  }
];

export default routes;