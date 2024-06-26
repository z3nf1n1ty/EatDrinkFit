﻿// Project: EatDrinkFit.Web
// File: Controllers/DashboardController.cs
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
using EatDrinkFit.Web.Models.Charts;
using EatDrinkFit.Web.Models.Entities;
using EatDrinkFit.Web.Models.Entities.Charts;
using EatDrinkFit.Web.Services.Charts;
using Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeZoneConverter;

namespace EatDrinkFit.Web.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IGlobalProperties _globalProperties;
        private readonly IDashboardChartDataService _dashboardChartDataService;

        public DashboardController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, IGlobalProperties globalProperties, IDashboardChartDataService dashboardChartDataService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _globalProperties = globalProperties;
            _dashboardChartDataService = dashboardChartDataService;
        }

        public async Task<IActionResult> Index()
        {
            // Get user timezone from browser cookie not form post data
            string userTimezone = TimezoneHelper.GetBrowserReportedTimezone(Request);

            // Create a DashboardViewModel to be consummed by the view.
            var dashboardViewModel = new DashboardViewModel();

            // Create a start date for building out chart data in the event of an error.
            var startDate = (TimezoneHelper.ConvertFromUTC_IANA(DateTime.UtcNow, userTimezone)).Date;            

            // 
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userID is null)
            {
                return NotFound(); // TODO: Change to an error page and message.
            }

            // Get calorie chart data and add it to the view model.
            dashboardViewModel = await _dashboardChartDataService.GetDashboardCalorieChartModels(dashboardViewModel, userID, startDate);

            // Get Macro chart data and add it to the view model.
            dashboardViewModel = await _dashboardChartDataService.GetDashboardMacroChartModels(dashboardViewModel, userID, startDate);

            // Get Micro chart data and add it to the view model.
            dashboardViewModel = await _dashboardChartDataService.GetDashboardMicroChartModels(dashboardViewModel, userID, startDate);

            // Get Percent Calorie chart data and add it to the view model.
            dashboardViewModel = await _dashboardChartDataService.GetDashboardPercentCalChartModels(dashboardViewModel, userID, startDate);

            return View(dashboardViewModel);
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

            // Replace default DateTime with DateTime.Now if needed. Expressed as the users local time.
            var timeStamp = ProcessDefaultDateTime(viewModel.TimeStamp, viewModel.ManualTZ);

            // Format timezone for databast per global properties.
            string timeZone;

            if ((bool)_globalProperties.Application["DatabaseTimezoneIANA"])
            {
                timeZone = viewModel.ManualTZ;
            }
            else
            {
                timeZone = TZConvert.IanaToWindows(viewModel.ManualTZ);
            }

            // Create the object for the database transaction.
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
                Source = MacroLogSource.Manual,
                IsFavorite = false, // Cannot be favorite when a manual entry is used.
                FromFavorites = false, // Cannot be from favorites when a manual entry is used.
                TimeStamp = timeStamp,
                Timezone = timeZone,
            };

            await _dbContext.MacroLogs.AddAsync(macroLog);

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

        /// <summary>
        /// Replace a provided default, aka min, DateTime object with the current local time specifed by the IANA timezone.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>A DateTime ojbect with the current local time specifed by the IANA timezone.</returns>
        private DateTime ProcessDefaultDateTime(DateTime source, string targetTimeZoneIANA)
        {
            var defaultDateTime = new DateTime();

            if (source == defaultDateTime)
            {
                var tzi = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZoneIANA);

                source = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);
            }

            return source;
        }
    }
}
