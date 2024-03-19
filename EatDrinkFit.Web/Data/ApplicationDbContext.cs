using EatDrinkFit.Web.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EatDrinkFit.Web.Data
{
	/*
     * Setup Database
     * 1 - Configure/apply database settings and configure tables defined by appsettings.json, ApplicationDbContext.cs and Program.cs
     * 2 - open package manager console (tools -> Nuget Package Manager -> Package Manager Console)
     * 3 - Run 'Add-Migration "Initial Migration"'
     * 4 - Run 'Update-Database'
     */

	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<MacroLog> MacroLogs { get; set; }
    }
}
