import Vue = require('vue');
import { LoadingState } from '../../main';
import { postsResource } from '../../helpers/resources';

export default Vue.extend({
  template: require('./posts.html'),

  data() {
    return {
      posts: []
    };
  },

  created(){
    (<any>this).fetchPosts();
  },

  methods: {
    fetchPosts(){
      LoadingState.$emit('toggle', true);
      return postsResource.get().then((response) => {
        (<any>this).posts = response.data;
        LoadingState.$emit('toggle', false);
      }, (errorResponse) => {
        
        console.log('API responded with:', errorResponse.status);
        LoadingState.$emit('toggle', false);
      });
    }
  }

});
