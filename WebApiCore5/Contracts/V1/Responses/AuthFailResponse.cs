using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore5.Contracts.V1.Responses
{
	public class AuthFailResponse
	{
		public IEnumerable<string> Errors { get; set; }
	}
}
