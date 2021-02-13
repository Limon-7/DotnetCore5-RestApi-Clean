﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Domain;

namespace WebApiCore5.Services
{
	public interface IIdentityService
	{
		Task<AuthenticationResult> RegisterAsync(string email, string password);
		Task<AuthenticationResult> LoginAsync(string email, string password);
	}
}