using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore5.Options
{
	public class JwtSettings
	{
		public string Secret { get; set; }
		public TimeSpan TokenLifeTime { get; internal set; }
	}
}
