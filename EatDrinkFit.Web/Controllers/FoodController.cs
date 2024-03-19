// Project: EatDrinkFit.Web
// File: Controllers/FoodController.cs
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
using EatDrinkFit.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EatDrinkFit.Web.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class FoodController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public FoodController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //return View();
            return RedirectToAction("Meals");
        }

        [HttpGet]
        public IActionResult Favorites()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Meals()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Components()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Ingredients()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Manual()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Manual(ManualMacroLogViewModel viewModel)
        {
            // TODO: Verify the data before submitting to the database as well as to clear the model.

            var macroLog = new MacroLog
            {
                UserId = _userManager.GetUserId(this.User),
                Calories = viewModel.Calories,
                Fat = viewModel.Fat,
                Cholesterol = viewModel.Cholesterol,
                Sodium = viewModel.Sodium,
                TotalCarb = viewModel.TotalCarb,
                Fiber = viewModel.Fiber,
                Sugar = viewModel.Sugar,
                Protein = viewModel.Protein,
                Note = viewModel.Note,
                TimeStamp = viewModel.TimeStamp,
            };

            await _dbContext.MacroLogs.AddAsync(macroLog);

            await _dbContext.SaveChangesAsync();

            ModelState.Clear();

            return View("Manual");
        }
    }
}
