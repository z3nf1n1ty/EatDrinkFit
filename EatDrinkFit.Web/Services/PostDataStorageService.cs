// Project: EatDrinkFit.Web
// File: Services/Charts/DashboardChartDataService.cs
// Origonially designed for ASP.NET Core 8.0

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// Copyright (C) 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor:

// Services use a service installer...

using EatDrinkFit.Web.Configuration;
using EatDrinkFit.Web.Data;
using EatDrinkFit.Web.Helpers;
using EatDrinkFit.Web.Models;
using EatDrinkFit.Web.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using TimeZoneConverter;

namespace EatDrinkFit.Web.Services.Charts
{
    public interface IPostDataStorageService
    {
        /// <summary>
        /// Processes the provided post data and store it in the db.
        /// </summary>
        /// <returns>True for success of proccessing and storage.</returns>
        public Task<bool> ProcessManualFoodPostData(ManualMacroLogViewModel viewModel, string? userID);
    }

    /// <summary>
    /// Privoid logic, processing, storage of user/form data.
    /// </summary>
    public sealed class PostDataStorageService : IPostDataStorageService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IGlobalProperties _globalProperties;

        public PostDataStorageService(ApplicationDbContext dbContext, IGlobalProperties globalProperties)
        {
            _dbContext = dbContext;
            _globalProperties = globalProperties;
        }

        /// <summary>
        /// Processes the provided post data and store it in the db.
        /// </summary>
        /// <returns>True for success of proccessing and storage.</returns>
        public async Task<bool> ProcessManualFoodPostData(ManualMacroLogViewModel viewModel, string? userID)
        {
            // Replace default DateTime with DateTime.Now if needed. Expressed as the users local time.
            var timeStamp = TimezoneHelper.ProcessDefaultDateTime(viewModel.TimeStamp, viewModel.ManualTZ);

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
                UserId = userID,
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

            // TODO: verify good db state change or failure?

            await _dbContext.MacroLogs.AddAsync(macroLog);

            await _dbContext.SaveChangesAsync();

            return true;
        }
    }    
}
