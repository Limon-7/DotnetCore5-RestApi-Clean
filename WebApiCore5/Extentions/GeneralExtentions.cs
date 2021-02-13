using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore5.Extentions
{
	public static class GeneralExtentions
	{
		/// <summary>
		/// / This method will return UserId from the token. Because in the jwt claim id is bound in the token.
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		#region GetUserId from token using HttpContext
		public static string GetUserId(this HttpContext httpContext)
		{
			if (httpContext.User == null)
				return string.Empty;

			return httpContext.User.Claims.Single(x => x.Type == "id").Value;
		}
		#endregion


	}
}
