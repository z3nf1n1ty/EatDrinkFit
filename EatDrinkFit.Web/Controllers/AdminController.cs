﻿// Project: EatDrinkFit.Web
// File: AdminController.cs
// Origonially designed for ASP.NET Core 8.0

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// Copyright (C) 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor:

using EatDrinkFit.Web.Data;
using EatDrinkFit.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EatDrinkFit.Web.Controllers
{
    //[Authorize(Roles = "Administrator")]
	public class AdminController : Controller
	{
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
                await _dbContext.Database.MigrateAsync();

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