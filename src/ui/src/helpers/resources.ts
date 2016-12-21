import Vue = require('vue');
import VueResource = require('vue-resource');
import { router } from '../main';

const API_BASE = 'http://jsonplaceholder.typicode.com';

Vue.use(VueResource);

(<any>Vue).http.options = {
  root: API_BASE
};

(<any>Vue).http.interceptors.push((request, next) => {
  next((response) => {
    // Handle global API 404 =>
    if (response.status === 404) {
      router.push('/404');
    }
  });
});

export const postsResource = (<any>Vue).resource('posts{/id}');
