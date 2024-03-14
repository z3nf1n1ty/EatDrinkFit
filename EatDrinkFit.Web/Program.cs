// Project: EatDrinkFit.Web
// File: Program.cs
// Origonially designed for ASP.NET Core 8.0

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor: 

using EatDrinkFit.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();
	//.AddDefaultTokenProviders
builder.Services.AddControllersWithViews();

// Was in the Example, and missing from the base template.
builder.Services.AddRazorPages();

// JWT support for authentication (jwt.io)
//builder.Services.AddAuthentication().AddJwtBearer();

/*
 * The preceding highlighted code sets the fallback authorization policy. The fallback authorization policy requires all 
 * users to be authenticated, except for Razor Pages, controllers, or action methods with an authorization attribute. 
 * 
 * The fallback authorization policy:
 * Is applied to all requests that don't explicitly specify an authorization policy. For requests served by endpoint routing, 
 * this includes any endpoint that doesn't specify an authorization attribute. For requests served by other middleware after 
 * the authorization middleware, such as static files, this applies the policy to all requests.
 * 
 * Setting the fallback authorization policy to require users to be authenticated protects newly added Razor Pages and 
 * controllers. Having authorization required by default is more secure than relying on new controllers and Razor Pages 
 * to include the [Authorize] attribute.
 */
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

////dev code add a break point and can spy on the context of incoming requests.
//app.Use(async (context, next) =>
//	{
//		await next(context);
//	});

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
