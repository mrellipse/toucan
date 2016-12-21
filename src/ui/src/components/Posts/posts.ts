import Vue = require('vue');
import Component from 'vue-class-component';
import { LoadingState } from '../../main';
import { postsResource } from '../../helpers/resources';
import { IPost } from '../../model/post';

@Component({
  template: require('./posts.html')
})
class Posts extends Vue {

  posts: IPost[] = [];

  created() {
    this.fetchPosts();
  }

  fetchPosts() {
    LoadingState.$emit('toggle', true);

    let current = this;

    let onSuccess = (response) => {
      this.posts = response.data;
      LoadingState.$emit('toggle', false);
    };

    let onFailure = (errorResponse) => {
      console.log('API responded with:', errorResponse.status);
      LoadingState.$emit('toggle', false);
    };

    return postsResource.get().then(onSuccess, onFailure);
  }
}

export default Posts;
