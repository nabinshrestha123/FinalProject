using Microsoft.EntityFrameworkCore;
using ProductAndOrder.Domain;
using ProductAndOrder.Domain.Entities;

namespace ProductAndOrder.Infrastructure.Data
{


	public class AppDBContext : DbContext
	{
		public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
		{
		}
	    public DbSet<Product> Products { get; set; }
		public DbSet<Order> Orders { get; set; }
	}
	}

