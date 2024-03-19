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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EatDrinkFit.Web.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class FoodController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public FoodController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
