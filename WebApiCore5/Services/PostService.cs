using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Data;
using WebApiCore5.Domain;

namespace WebApiCore5.Services
{
	public class PostService : IPostService
	{
		private readonly DataContext _context;

		public PostService(DataContext context)
		{
			_context = context;
		}

		public async Task<List<Post>> GetPostsAsync()
		{
			return await _context.Posts.ToListAsync();
		}

		public async Task<Post> GetPostByIdAsync(Guid postId)
		{
			return await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
		}

		public async Task<bool> CreatePostAsync(Post post)
		{
			await _context.Posts.AddAsync(post);
			var created = await _context.SaveChangesAsync();
			return created > 0;
		}

		public async Task<bool> UpdatePostAsync(Post postToUpdate)
		{
			_context.Posts.Update(postToUpdate);
			var update = await _context.SaveChangesAsync();
			return update>0;
		}

		public  async Task<bool> DeletePostAsync(Guid postId)
		{
			var post = await GetPostByIdAsync(postId);
			_context.Remove(post);
			var deleted = await _context.SaveChangesAsync();
			return deleted > 0;
		}

		public async Task<bool> UserOwnPostAsync(Guid postId, string userId)
		{
			var post =await _context.Posts.SingleOrDefaultAsync(x => x.Id == postId);
			if (post == null)
			{
				return false;
			}

			if (post.UserId != userId)
			{
				return false;
			}
			return true;
		}
	}
}
