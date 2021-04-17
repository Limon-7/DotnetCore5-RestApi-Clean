using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebApiCore5.Domain;

namespace WebApiCore5.Data
{
	public class DataContext : IdentityDbContext
	{
		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{
		}
		public DbSet<Post> Posts { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
	}
}
