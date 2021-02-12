using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Services;

namespace WebApiCore5.Installers
{
	public class MvcInstaller : IInstaller
	{
		public void InstallServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(x => {
				x.SwaggerDoc("v1", new OpenApiInfo { Title = "My sweet app", Version = "V1" });
			});
			
		}
	}
}
