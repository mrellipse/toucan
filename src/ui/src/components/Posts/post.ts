import Vue = require('vue');
import Component from 'vue-class-component';
import { postsResource } from '../../helpers/resources';
import { LoadingState } from '../../main';
import { IPost } from '../../model/post';

@Component({
  name: 'Post',
  template: require('./post.html')
})
class Post extends Vue {

  post: IPost = { id: 1, userId: 0, title: '', body: '' };

  created() {
    this.fetchPost();
  }

  fetchPost() {
    const id: string = this.$route.params['id'];
    
    LoadingState.$emit('toggle', true);

    return postsResource.get({ id }).then((response) => {

      Object.assign(this.post, response.data);

      LoadingState.$emit('toggle', false);
    }, (errorResponse) => {
      console.log('API responded with:', errorResponse.status);
      LoadingState.$emit('toggle', false);
    });
  }
}

export default Post;