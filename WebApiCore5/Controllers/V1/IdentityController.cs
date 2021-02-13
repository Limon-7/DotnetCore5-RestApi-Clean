using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Contracts.V1;
using WebApiCore5.Contracts.V1.Requests;
using WebApiCore5.Contracts.V1.Responses;
using WebApiCore5.Services;

namespace WebApiCore5.Controllers.V1
{
	public class IdentityController:Controller
	{
		private readonly IIdentityService _identityService;

		public IdentityController(IIdentityService identityService)
		{
			_identityService = identityService;
		}
		   [HttpPost(ApiRoutes.Identity.Register)]
		public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new AuthFailResponse
				{
					Errors = ModelState.Values.SelectMany(x=>x.Errors.Select(xx=>xx.ErrorMessage))
				});
			}
			var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);
			if (!authResponse.Success)
			{
				return BadRequest(new AuthFailResponse
				{
				  Errors=authResponse.Errors
				});
			}
			return Ok(new AuthSuccessResponse { 
				Token=authResponse.Token
			});
		}
		[HttpPost(ApiRoutes.Identity.Login)]
		public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
		{
			var authResponse = await _identityService.LoginAsync(request.Email, request.Password);
			if (!authResponse.Success)
			{
				return BadRequest(new AuthFailResponse
				{
					Errors = authResponse.Errors
				});
			}
			return Ok(new AuthSuccessResponse
			{
				Token = authResponse.Token
			});
		}
	}
}
