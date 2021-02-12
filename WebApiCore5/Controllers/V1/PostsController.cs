using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Contracts.V1;
using WebApiCore5.Contracts.V1.Requests;
using WebApiCore5.Contracts.V1.Responses;
using WebApiCore5.Domain;
using WebApiCore5.Services;

namespace WebApiCore5.Controllers.V1
{
	public class PostsController : Controller
	{
		private readonly IPostService _postService;

		public PostsController(IPostService postService)
		{
			_postService = postService;
		}

		[HttpGet(ApiRoutes.Post.GetAll)]
		public IActionResult GetAll()
		{
			return Ok(_postService.GetPosts());
		}

		[HttpGet(ApiRoutes.Post.Get)]
		public IActionResult Get([FromRoute] Guid postId)
		{
			var post =_postService.GetPostById(postId);
			if (post == null)
				return NotFound();
			return Ok(post);
		}
		[HttpPut(ApiRoutes.Post.Update)]
		public IActionResult Update([FromRoute] Guid postId,UpdatePostRequest request)
		{

			var post = new Post { Id = postId, Name = request.Name };
			var update = _postService.UpdatePost(post);
			if (update)
				return Ok(post);
			return NotFound(post);
		}


		[HttpDelete(ApiRoutes.Post.Delete)]
		public IActionResult Delete([FromRoute] Guid postId)
		{
			var post = _postService.DeletePost(postId);
			if (post)
				return NoContent();
			return NotFound();
		}


		[HttpPost(ApiRoutes.Post.Create)]
		public IActionResult Create([FromBody] CreatePostRequest postRequest)
		{
			var post = new Post { Id = postRequest.Id };
			if (post.Id!=Guid.Empty)
				post.Id = Guid.NewGuid();

			_postService.GetPosts().Add(post);
			var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
			Console.WriteLine($"baseUrl:{baseUrl}");
			var locationUrl = baseUrl + "/" + ApiRoutes.Post.Get.Replace("{postId}", post.Id.ToString());
			Console.WriteLine($"locationUrl:{locationUrl}");
			var response = new PostResponse { Id = post.Id };
			return Created(locationUrl,response);
		}


	}
}
