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
using EatDrinkFit.Web.Models.Entities.Charts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Drawing.Text;
using TimeZoneConverter;

namespace EatDrinkFit.Web.Services.Charts
{
    public interface IDashboardChartDataService
    {
        /// <summary>
        /// Processes the provided post data and stores it in the db.
        /// </summary>
        /// <returns>True for success of proccessing and storage.</returns>
        public Task<bool> ProcessChartDataAfterMacroLogUpdate(string? userID);
    }

    /// <summary>
    /// Privoid logic, processing, storage, and retrieval of chart data.
    /// </summary>
    public sealed class DashboardChartDataService : IDashboardChartDataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IGlobalProperties _globalProperties;

        private class TransactionMacroLog
        {
            public DateTime TimeStamp { get; set; }

            public string? Timezone { get; set; }

            public uint Calories { get; set; }

            public float Fat { get; set; }

            public float Cholesterol { get; set; }

            public float Sodium { get; set; }

            public float TotalCarb { get; set; }

            public float Fiber { get; set; }

            public float Sugar { get; set; }

            public float Protein { get; set; }
        }

        public DashboardChartDataService(ApplicationDbContext dbContext, IGlobalProperties globalProperties)
        {
            _dbContext = dbContext;
            _globalProperties = globalProperties;
        }

        /// <summary>
        /// Processes the data from the db to update and refresh chart data related to Macro Logs.
        /// </summary>
        /// <returns>True for success of proccessing and storage.</returns>
        public async Task<bool> ProcessChartDataAfterMacroLogUpdate(string? userID, string userTimezone)
        {
            /* List of Chart Data that need to be updated:
             * Dashboard - Calories
             * todo Dashboard - macros
             * todo Dashboard - daily percent of calories
             */

            var startDate = TimezoneHelper.ConvertFromUTC_IANA(DateTime.UtcNow, userTimezone);

            // Use a transation to make the retreval and update of data.
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            // Get the dashboard chart data needed from the MacroLogs as a list.
            var macroLogs = await _dbContext.MacroLogs
                .AsNoTracking()
                .Where(l => l.UserId == userID)
                .Select(l => new TransactionMacroLog()
                {
                    TimeStamp = l.TimeStamp,
                    Timezone = l.Timezone,
                    Calories = l.Calories,
                    Fat = l.Fat,
                    Cholesterol = l.Cholesterol,
                    Sodium = l.Sodium,
                    TotalCarb = l.TotalCarb,
                    Fiber = l.Fiber,
                    Sugar = l.Sugar,
                    Protein = l.Protein,
                })
                .ToListAsync();

            if (macroLogs is null)
            {
                await transaction.RollbackAsync();

                // TODO: Log the db error.

                return false;
            }

            // Proccess the macro data into the calorie chart data.
            foreach ( var macroLog in macroLogs )
            {
                
            }

            // Store the calorie chart data.
             
            // Process the macro data into the macro chart data.

            // Store the macro chart data.

            // Save the complete set of changes to the database.
            await transaction.CommitAsync();

            return true;
        }

        private DashboardCalorieChartEntry CreateDashboardCalorieChartEntry(TransactionMacroLog transactionMacroLog, string? userID, DateOnly startDate)
        {
            var logDate = new DateTime();
            uint calories = 0;

            var dcce = new DashboardCalorieChartEntry()
            {
                // (key) id is left null to be generated by the db
                UserId = userID,
                LogDate = logDate,
                Calories = calories,
            };

            return dcce;
        }
    }    
}
