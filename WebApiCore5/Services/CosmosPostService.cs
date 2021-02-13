using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Domain;

namespace WebApiCore5.Services
{
	public class CosmosPostService : IPostService
	{
		public Task<bool> CreatePostAsync(Post post)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeletePostAsync(Guid postId)
		{
			throw new NotImplementedException();
		}

		public Task<Post> GetPostByIdAsync(Guid postId)
		{
			throw new NotImplementedException();
		}

		public Task<List<Post>> GetPostsAsync()
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdatePostAsync(Post postToUpdate)
		{
			throw new NotImplementedException();
		}

		public Task<bool> UserOwnPostAsync(Guid postId, string userId)
		{
			throw new NotImplementedException();
		}
	}
}
