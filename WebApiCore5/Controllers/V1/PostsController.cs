using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Contracts.V1;
using WebApiCore5.Contracts.V1.Requests;
using WebApiCore5.Contracts.V1.Responses;
using WebApiCore5.Data;
using WebApiCore5.Domain;
using WebApiCore5.Extentions;
using WebApiCore5.Services;

namespace WebApiCore5.Controllers.V1
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class PostsController : Controller
	{
		private readonly IPostService _postService;

		public PostsController( IPostService postService)
		{
			_postService = postService;
		}

		[HttpGet(ApiRoutes.Post.GetAll)]
		public async Task<IActionResult> GetAll()
		{
			return Ok(await _postService.GetPostsAsync());
		}

		[HttpGet(ApiRoutes.Post.Get)]
		public  async Task<IActionResult> Get([FromRoute] Guid postId)
		{
			var post =await _postService.GetPostByIdAsync(postId);
			if (post == null)
				return NotFound();
			return Ok(post);
		}
		[HttpPut(ApiRoutes.Post.Update)]
		public async Task<IActionResult> Update([FromRoute] Guid postId,UpdatePostAsyncRequest request)
		{
			var userOwnPosts = await _postService.UserOwnPostAsync(postId,HttpContext.GetUserId());
			if (!userOwnPosts)
			{
				return BadRequest(new {error="You do not own this posts" });
			}
			var post = await _postService.GetPostByIdAsync(postId);
			post.Name = request.Name;
			var update = await _postService.UpdatePostAsync(post);
			if (update)
				return Ok(post);
			return NotFound(post);
		}


		[HttpDelete(ApiRoutes.Post.Delete)]
		public async Task<IActionResult> Delete([FromRoute] Guid postId)
		{
			var userOwnPosts = await _postService.UserOwnPostAsync(postId, HttpContext.GetUserId());
			if (!userOwnPosts)
			{
				return BadRequest(new { error = "You do not own this posts" });
			}
			var post = await _postService.DeletePostAsync(postId);
			if (post)
				return NoContent();
			return NotFound();
		}


		[HttpPost(ApiRoutes.Post.Create)]
		public async Task<IActionResult> Create([FromBody] CreatePostAsyncRequest postRequest)
		{
			var post = new Post
			{
				Name = postRequest.Name,
				UserId = HttpContext.GetUserId()
			};
			

			await _postService.CreatePostAsync(post);
			var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
			Console.WriteLine($"baseUrl:{baseUrl}");
			var locationUrl = baseUrl + "/" + ApiRoutes.Post.Get.Replace("{postId}", post.Id.ToString());
			Console.WriteLine($"locationUrl:{locationUrl}");
			var response = new PostResponse { Id = post.Id,Name=post.Name };
			return Created(locationUrl,response);
		}


	}
}
