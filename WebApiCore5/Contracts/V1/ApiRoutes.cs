using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiCore5.Contracts.V1
{
	public static class ApiRoutes
	{
		public const string Root = "api";
		public const string Version = "v1";
		public const string Base = Root+"/"+Version;
		public static class Test
		{
			public const  string GetAll = Base+"/test";
		}
		public static class Post
		{
			public const string GetAll = Base + "/post";
			public const string Get = Base + "/post/{postId}";
			public const string Update = Base + "/post/{postId}";
			public const string Delete = Base + "/post/{postId}";
			public const string Create = Base + "/post";
			
		}
	}
}
