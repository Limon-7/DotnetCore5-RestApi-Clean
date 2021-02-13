using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiCore5.Domain;
using WebApiCore5.Options;

namespace WebApiCore5.Services
{
	public class IdentityService : IIdentityService
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly JwtSettings _jwtSettings;

		public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings)
		{
			_userManager = userManager;
			_jwtSettings = jwtSettings;
		}



		public async Task<AuthenticationResult> RegisterAsync(string email, string password)
		{
			var existingUser = await _userManager.FindByEmailAsync(email);
			// first check user	- if found then
			if (existingUser != null)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "The user with this mail already in exists " }
				};
			}
			//if not found then create object
			var user = new IdentityUser
			{
				Email = email,
				UserName = email
			};
			var createUser = await _userManager.CreateAsync(user, password);
			Console.WriteLine($"createUser: {createUser}");
			if (!createUser.Succeeded)
			{
				return new AuthenticationResult
				{
					Errors = createUser.Errors.Select(x => x.Description)
				};
			}
			return GenerateJwtForUser(user);

		}
		public async Task<AuthenticationResult> LoginAsync(string email, string password)
		{
			var user = await _userManager.FindByEmailAsync(email);
			// first check user	- if found then
			if (user == null)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "User does not exist " }
				};
			}
			var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
			if (!userHasValidPassword)
			{
				return new AuthenticationResult
				{
					Errors = new[] { "User/password combination is wrong " }
				};
			}
			return GenerateJwtForUser(user);

		}
		private AuthenticationResult GenerateJwtForUser(IdentityUser user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
			// Describe what our token should have
			var tokenDiscriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.Email),
					// jti=Unique id for specific jwt
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(JwtRegisteredClaimNames.Email, user.Email),
					// custom claim
					new Claim("id", user.Id),
				}),
				Expires = DateTime.UtcNow.AddHours(2),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDiscriptor);
			return new AuthenticationResult
			{
				Success = true,
				Token = tokenHandler.WriteToken(token)
			};
		}
	}
}
