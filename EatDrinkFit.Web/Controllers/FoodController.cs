﻿// Project: EatDrinkFit.Web
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

using EatDrinkFit.Web.Configuration;
using EatDrinkFit.Web.Data;
using EatDrinkFit.Web.Helpers;
using EatDrinkFit.Web.Models;
using EatDrinkFit.Web.Models.Entities;
using EatDrinkFit.Web.Services.Charts;
using Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeZoneConverter;

namespace EatDrinkFit.Web.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class FoodController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IGlobalProperties _globalProperties;
        private readonly IPostDataStorageService _postDataStorageService;
        private readonly IDashboardChartDataService _dashboardChartDataService;

        public FoodController(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            IGlobalProperties globalProperties,
            IPostDataStorageService postDataStorageService,
            IDashboardChartDataService dashboardChartDataService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _globalProperties = globalProperties;
            _postDataStorageService = postDataStorageService;
            _dashboardChartDataService = dashboardChartDataService;
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
        public async Task<IActionResult> ManualFood(ManualMacroLogViewModel viewModel)
        {
            // TODO: Verify the data before submitting to the database as well as to clear the model.

            // Get userID for this context.
            var userID = _userManager.GetUserId(this.User);

            // Get user timezone from browser cookie not form post data
            string userTimezone = TimezoneHelper.GetBrowserReportedTimezone(Request);

            // Process the verified form data for the manual food post using the service.
            if (await _postDataStorageService.ProcessManualFoodPostData(viewModel, userID) is false)
            {
                // Error

                // TODO: Return to the view with the form data still pressent, and a error notification.
            }

            // Chart data will need to be updated if the previous was successfull.
            if (await _dashboardChartDataService.ProcessChartDataAfterMacroLogUpdate(userID, userTimezone) is false)
            {
                // Error

                // TODO: Return to the view with the form data cleared, and a error notification.

                ModelState.Clear();

                return RedirectToAction("Manual");
            }


            //// Replace default DateTime with DateTime.Now if needed. Expressed as the users local time.
            //var timeStamp = ProcessDefaultDateTime(viewModel.TimeStamp, viewModel.ManualTZ);

            //// Format timezone for databast per global properties.
            //string timeZone;

            //if ((bool)_globalProperties.Application["DatabaseTimezoneIANA"])
            //{
            //    timeZone = viewModel.ManualTZ;
            //}
            //else
            //{
            //    timeZone = TZConvert.IanaToWindows(viewModel.ManualTZ);
            //}

            //// Create the object for the database transaction.
            //var macroLog = new MacroLog
            //{
            //    UserId = _userManager.GetUserId(this.User),
            //    Calories = viewModel.Calories,
            //    Fat = viewModel.Fat,
            //    Cholesterol = viewModel.Cholesterol,
            //    Sodium = viewModel.Sodium,
            //    TotalCarb = viewModel.TotalCarb,
            //    Fiber = viewModel.Fiber,
            //    Sugar = viewModel.Sugar,
            //    Protein = viewModel.Protein,
            //    Note = viewModel.Note,
            //    Source = MacroLogSource.Manual,
            //    IsFavorite = false, // Cannot be favorite when a manual entry is used.
            //    FromFavorites = false, // Cannot be from favorites when a manual entry is used.
            //    TimeStamp = timeStamp,
            //    Timezone = timeZone,
            //};

            //await _dbContext.MacroLogs.AddAsync(macroLog);

            //await _dbContext.SaveChangesAsync();

            // The post has been successfully processed, stored, and charts updated. Clear the form and notify success.

            ModelState.Clear();

            return RedirectToAction("Manual");  // TODO: Notify success
        }

        [HttpPost]
        public async Task<IActionResult> ManualWater(ManualMacroLogViewModel viewModel)
        {
            // TODO: Verify the data before submitting to the database as well as to clear the model.

            // Replace default DateTime with DateTime.Now if needed. Expressed as the users local time.
            var timeStamp = TimezoneHelper.ProcessDefaultDateTime(viewModel.TimeStamp, viewModel.WaterTZ);

            // Format timezone for databast per global properties.
            string timeZone;

            if ((bool)_globalProperties.Application["DatabaseTimezoneIANA"])
            {
                timeZone = viewModel.WaterTZ;
            }
            else
            {
                timeZone = TZConvert.IanaToWindows(viewModel.WaterTZ);
            }

            // Create the object for the database transaction.
            var hydrationLog = new HydrationLog
            {
                UserId = _userManager.GetUserId(this.User),
                Unit = viewModel.WaterUnit,
                Ammount = viewModel.WaterAmmount,
                Note = viewModel.WaterNote,
                TimeStamp = timeStamp,
                Timezone = timeZone,
                Type = HydrationLogType.Water,
                Source = HydrationLogSource.Manual,
                FromFavorites = false,
                IsFavorite = false,
            };

            await _dbContext.HydrationLogs.AddAsync(hydrationLog);

            await _dbContext.SaveChangesAsync();

            ModelState.Clear();

            return RedirectToAction("Manual");
        }

        [HttpPost]
        public async Task<IActionResult> ManualFluid(ManualMacroLogViewModel viewModel)
        {
            // TODO: Verify the data before submitting to the database as well as to clear the model.

            // Replace default DateTime with DateTime.Now if needed. Expressed as the users local time.
            var timeStamp  = TimezoneHelper.ProcessDefaultDateTime(viewModel.TimeStamp, viewModel.FluidTZ);

            // Format timezone for databast per global properties.
            string timeZone;

            if ((bool)_globalProperties.Application["DatabaseTimezoneIANA"])
            {
                timeZone = viewModel.FluidTZ;
            }
            else
            {
                timeZone = TZConvert.IanaToWindows(viewModel.FluidTZ);
            }

            // Create the object for the database transaction.
            var hydrationLog = new HydrationLog
            {
                UserId = _userManager.GetUserId(this.User),
                Unit = viewModel.FluidUnit,
                Ammount = viewModel.FluidAmmount,
                Note = viewModel.FluidNote,
                TimeStamp = timeStamp,
                Timezone = timeZone,
                Type = HydrationLogType.Fluid,
                Source = HydrationLogSource.Manual,
                FromFavorites = false,
                IsFavorite = false,
            };

            await _dbContext.HydrationLogs.AddAsync(hydrationLog);

            await _dbContext.SaveChangesAsync();

            ModelState.Clear();

            return RedirectToAction("Manual");
        }

        /// <summary>
        /// Replace a provided default, aka min, DateTime object with the current UTC time.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>A DateTime ojbect with the current UTC time.</returns>
        private DateTime ProcessDefaultDateTime(DateTime source)
        {
            var defaultDateTime = new DateTime();

            if(source == defaultDateTime)
            {
                source = DateTime.UtcNow;
            }

            return source;
        }
    }
}
