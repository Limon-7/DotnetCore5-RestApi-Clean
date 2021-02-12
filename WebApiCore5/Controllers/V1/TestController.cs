using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Contracts.V1;

namespace WebApiCore5.Controllers.V1
{
	public class TestController:Controller
	{
		[HttpGet(ApiRoutes.Test.GetAll)]
		public IActionResult GetAll()
		{
			return Ok(new { name = "limon" });
		}
	}
}
