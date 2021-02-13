using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiCore5.Options;
using WebApiCore5.Services;

namespace WebApiCore5.Installers
{
	public class MvcInstaller : IInstaller
	{
		public void InstallServices(IConfiguration configuration, IServiceCollection services)
		{
			#region Jwt configuration
			var jwtSettings = new JwtSettings();
			//JwtSettings will autometically map appsetting json to jwtSettings. 
			configuration.Bind(nameof(JwtSettings), jwtSettings);
			services.AddSingleton(jwtSettings);
			#endregion

			services.AddScoped<IIdentityService, IdentityService>();
			services.AddControllers();

			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(
					  x =>
					  {
						  x.SaveToken = true;
						  x.TokenValidationParameters = new TokenValidationParameters
						  {
							  ValidateIssuerSigningKey = true,
							  IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
							  ValidateIssuer = false,
							  ValidateAudience = false,
							  RequireExpirationTime = false,
							  ValidateLifetime = true
						  };
					  }
				);


			services.AddSwaggerGen(x =>
			{
				x.SwaggerDoc("v1", new OpenApiInfo { Title = "My sweet app", Version = "V1" });
				//var security = new Dictionary<string, IEnumerable<string>> {
				//	{"Bearer" ,new string[0]}
				//};
				x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme

				{
					Description = "JWT Authorisation header using the bearer scheme",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey
				});

				x.AddSecurityRequirement(new OpenApiSecurityRequirement() {

				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						},
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header,
					},
					new List<string>()
				}
				});
			});

		}
	}
}
