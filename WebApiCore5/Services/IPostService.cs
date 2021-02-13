using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Domain;

namespace WebApiCore5.Services
{
	public interface IPostService
	{
		Task<List<Post>> GetPostsAsync();
		Task<Post> GetPostByIdAsync(Guid postId);
		Task<bool> CreatePostAsync(Post post);
		Task<bool> UpdatePostAsync(Post postToUpdate);
		Task<bool> DeletePostAsync(Guid postId);
		Task<bool> UserOwnPostAsync(Guid postId, string userId);
	}
}
