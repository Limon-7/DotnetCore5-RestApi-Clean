using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Domain;

namespace WebApiCore5.Services
{
	public interface IPostService
	{
		List<Post> GetPosts();
		Post GetPostById(Guid postId);
		bool UpdatePost(Post postToUpdate);
		bool DeletePost(Guid postId);
	}
}
