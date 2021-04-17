using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiCore5.Data;
using WebApiCore5.Domain;
using WebApiCore5.Options;

namespace WebApiCore5.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, DataContext context)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
        }


        #region RegisterAsync
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
            return await GenerateJwtForUserAync(user);

        }
        #endregion RegisterAsync


        #region LoginAsync
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
            return await GenerateJwtForUserAync(user);

        }
        #endregion


        #region RefreshToken
        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            //check token
            var validateToken = GetPrincipleFromToken(token);
            if (validateToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid Token" }
                };
            }
            // check is token expeired
            var expiryDateUnix = long.Parse(validateToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);
            //.Subtract(_jwtSettings.TokenLifeTime);
            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Your token is not expire yet" }
                };
            }
            // get token Id
            var jti = validateToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token.ToString() == refreshToken);
            if (storedRefreshToken == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Your refresh does not exist" }
                };
            }
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Your refresh has expired" }
                };
            }
            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "The refresh token has been invalidated" }
                };
            }
            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "The refresh token has been used" }
                };
            }
            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "The refresh token does not exist" }
                };
            }
            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validateToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateJwtForUserAync(user);

        }
        #endregion

        /// <summary>
        /// ClaimPrinciple ClaimsPrincipal contains just a collection of identities and points 
        /// to the currently used one but as far as I know, the principal usually never contains 
        /// more than 1 identity and even if it would - the user is never logged in with 2 or more identities
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        #region GetPrincipleFromToken
        private ClaimsPrincipal GetPrincipleFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principle = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validateToken);
                // need to cheack valid security algorithom
                if (!JwtWithValidSecurityAlgorithm(validateToken))
                {
                    return null;
                }
                return principle;
            }
            catch
            {
                return null;
            }
        }
        #endregion
        /// <summary>
        /// Check token is valid Security Algorithm
        /// </summary>
        /// <param name="validateToken"></param>
        /// <returns></returns>
        #region JwtWithValidSecurityAlgorithm
        private bool JwtWithValidSecurityAlgorithm(SecurityToken validateToken)
        {
            return (validateToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.
                Equals(SecurityAlgorithms.HmacSha256, StringComparison.CurrentCultureIgnoreCase);
        }
        #endregion

        #region GenerateJwtForUserAync
        private async Task<AuthenticationResult> GenerateJwtForUserAync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            Console.WriteLine($"time:{_jwtSettings.TokenLifeTime}");
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
                //Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                Expires = DateTime.UtcNow.AddSeconds(45),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDiscriptor);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
        #endregion



    }
}
