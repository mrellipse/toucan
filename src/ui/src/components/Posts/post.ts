import Vue = require('vue');
import { postsResource } from '../../helpers/resources';
import { LoadingState } from '../../main';

export default Vue.extend({
  template : require('./post.html'),

  data() {
    return {
      post: {},
    };
  },

  created() {
    (<any>this).fetchPost();
  },

  methods: {
    fetchPost() {
      const id = (<any>this).$route.params.id;

      LoadingState.$emit('toggle', true);

      return postsResource.get({ id }).then((response) => {
        (<any>this).post = response.data;
        LoadingState.$emit('toggle', false);
      }, (errorResponse) => {
        // Handle error...
        console.log('API responded with:', errorResponse.status);
        LoadingState.$emit('toggle', false);
      });
    }
  }
});
