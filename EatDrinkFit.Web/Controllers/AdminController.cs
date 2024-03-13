using EatDrinkFit.Web.Data;
using EatDrinkFit.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EatDrinkFit.Web.Controllers
{
	public class AdminController : Controller
	{
        private readonly ApplicationDbContext dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        //https://localhost:8002/Admin/DatabaseMigrations
        public IActionResult DatabaseMigrations()
		{
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> DatabaseMigrations(PerformDatabaseMigrationsViewModel viewModel)
		{
			if (viewModel.Confirmed)
			{
                await dbContext.Database.MigrateAsync();

                //await dbContext.Database.EnsureCreatedAsync();
            }
			else
			{
                //return View();
            }

            return View(viewModel);
		}
	}
}
