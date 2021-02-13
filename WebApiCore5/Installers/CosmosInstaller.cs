﻿using Cosmonaut;
using Cosmonaut.Extensions.Microsoft.DependencyInjection;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiCore5.Domain;

namespace WebApiCore5.Installers
{
	public class CosmosInstaller
	{
		public void InstallServices(IConfiguration configuration, IServiceCollection services)
		{
			var cosmosStoreSettings = new CosmosStoreSettings(
				configuration["CosmosSettings:DatabaseName"],
				configuration["CosmosSettings:AccountUri"],
				configuration["CosmosSettings:AccountKey"],
				   new ConnectionPolicy { ConnectionMode=ConnectionMode.Direct,ConnectionProtocol=Protocol.Tcp}
				);
			services.AddCosmosStore<Post>(cosmosStoreSettings);
		}
	}
}