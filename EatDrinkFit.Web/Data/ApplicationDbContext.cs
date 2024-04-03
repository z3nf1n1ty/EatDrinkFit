// Project: EatDrinkFit.Web
// File: Data/ApplicationDbContext.cs
// Origonially designed for ASP.NET Core 8.0

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// Copyright (C) 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor:

using EatDrinkFit.Web.Models.Entities;
using EatDrinkFit.Web.Models.Entities.Charts;
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

    //PM> Add-Migration 001 -OutputDir "Data/Migrations"

    public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<MacroLog> MacroLogs { get; set; }

        public DbSet<HydrationLog> HydrationLogs { get; set; }

        public DbSet<DashboardCalorieChartEntry> DashboardCalorieChartEnteries { get; set; }

        public DbSet<DashboardMacroChartEntry> DashboardMacroChartEnteries { get; set; }

        public DbSet<DashboardMicroChartEntry> DashboardMicroChartEnteries { get; set; }
    }
}
