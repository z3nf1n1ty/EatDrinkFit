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
using EatDrinkFit.Web.Models.Charts;
using EatDrinkFit.Web.Models.Entities;
using EatDrinkFit.Web.Models.Entities.Charts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public Task<bool> ProcessChartDataAfterMacroLogUpdate(string? userID, string userTimezone);

        public Task<DashboardViewModel> GetDashboardCalorieChartModels(DashboardViewModel dashboardViewModel, string userID, DateTime startDate);

        public Task<DashboardViewModel> GetDashboardMacroChartModels(DashboardViewModel dashboardViewModel, string userID, DateTime startDate);
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

            var startDate = (TimezoneHelper.ConvertFromUTC_IANA(DateTime.UtcNow, userTimezone)).Date;

            // Use a transation to make the retreval and update of data.
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            // Get the dashboard chart data needed from the MacroLogs as a list.
            var macroLogs = await _dbContext.MacroLogs
                .AsNoTracking()
                .Where(l => l.UserId == userID)
                .OrderByDescending(l => l.TimeStamp)
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
            List<DashboardCalorieChartEntry> dcceList = CreateDashboardCalorieChartEntry(macroLogs, userID, startDate);

            // Store the calorie chart data.
            foreach (var r in dcceList)
            {
                if (r is not null)
                {
                    //_dbContext.DashboardCalorieChartEnteries.Update(e).Property(x => x.Id).IsModified = false;
                    if(await _dbContext.DashboardCalorieChartEnteries.AnyAsync(e => e == r))
                    {
                        _dbContext.DashboardCalorieChartEnteries.Update(r).Property(x => x.Id).IsModified = false;
                    }
                    else
                    {
                        //await _dbContext.DashboardCalorieChartEnteries.AddAsync(r);
                        _dbContext.DashboardCalorieChartEnteries.Add(r);
                        // Add should be local until the savechanges, since the identity does not need to be used.
                        // Therfore the local add should be faster by removing the additional task related job overhead that is not providing benifit.
                        // Ref: https://medium.com/@iamprovidence/addasync-in-ef-is-pure-evil-d31231d8f04e
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            // Process the macro data into the macro chart data.
            List<DashboardMacroChartEntry> dmaceList = CreateDashboardMacroChartEntry(macroLogs, userID, startDate);

            // Store the macro chart data.
            foreach (var r in dmaceList)
            {
                if (r is not null)
                {
                    if (await _dbContext.DashboardMacroChartEnteries.AnyAsync(e => e == r))
                    {
                        _dbContext.DashboardMacroChartEnteries.Update(r).Property(x => x.Id).IsModified = false;
                    }
                    else
                    {
                        //await _dbContext.DashboardMacroChartEnteries.AddAsync(r);
                        _dbContext.DashboardMacroChartEnteries.Add(r);
                        // Add should be local until the savechanges, since the identity does not need to be used.
                        // Therfore the local add should be faster by removing the additional task related job overhead that is not providing benifit.
                        // Ref: https://medium.com/@iamprovidence/addasync-in-ef-is-pure-evil-d31231d8f04e
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            // Save the complete set of changes to the database.
            await transaction.CommitAsync();

            return true;
        }

        /// <summary>
        /// Get the dashboard calorie chart data in the form of a list of DashbardCalorieChartModel stored in a DashboardViewModel.
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <param name="userID"></param>
        /// <param name="startDate"></param>
        /// <returns>A list of Calorie Chart Data staring with the most current date.</returns>
        public async Task<DashboardViewModel> GetDashboardCalorieChartModels(DashboardViewModel dashboardViewModel, string userID, DateTime startDate)
        {
            if(dashboardViewModel is null)
            {
                dashboardViewModel = new DashboardViewModel();
            }

            int chartDays = (int)_globalProperties.Application["DashboardCalorieChartDays"];
            var chartDate = new DateTime(startDate.Ticks);
            var tempDate = chartDate.AddDays(-1 * chartDays);

            dashboardViewModel.DashboardCalorieChartModels = new List<Models.Charts.DashboardCalorieChartModel>(chartDays);

            var dashboardCalorieChartEntries = await _dbContext.DashboardCalorieChartEnteries
                                                 .Where(e => e.UserId == userID && e.LogDate > tempDate)
                                                 .OrderByDescending(e => e.LogDate)
                                                 .Select(e => new { e.Id, e.LogDate, e.Calories })
                                                 .ToListAsync();

            if (dashboardCalorieChartEntries is not null)
            {
                int i = 0;

                while (chartDate > tempDate)
                {
                    // Check if the current index in the list matches the current logDate.
                    if (i < dashboardCalorieChartEntries.Count && dashboardCalorieChartEntries[i].LogDate.Date == chartDate.Date)
                    {
                        dashboardViewModel.DashboardCalorieChartModels.Add(new DashboardCalorieChartModel()
                        {
                            Id = dashboardCalorieChartEntries[i].Id,
                            //Date = dashboardCalorieChartEntries[i].LogDate.DayOfWeek.ToString(),
                            Date = dashboardCalorieChartEntries[i].LogDate.ToString("ddd, dd MMM"),
                            Calories = dashboardCalorieChartEntries[i].Calories,
                        });

                        i++;
                    }
                    else
                    {
                        // No record for this day. Create default entry for this date.
                        dashboardViewModel.DashboardCalorieChartModels.Add(new DashboardCalorieChartModel()
                        {
                            Id = 0,
                            Date = chartDate.ToString("ddd, dd MMM"),
                            Calories = 0,
                        });
                    }                   

                    chartDate = chartDate.AddDays(-1);
                }
            }
            else
            {
                // No valid data from the database, make placeholder data.
                for (int i = 0; i < chartDays; i++)
                {
                    dashboardViewModel.DashboardCalorieChartModels.Add(new DashboardCalorieChartModel()
                    {
                        Id = 0,
                        Date = chartDate.DayOfWeek.ToString(),
                        Calories = 0,
                    });

                    chartDate = chartDate.AddDays(-1);
                }

                // TODO: Create a text entry to display on the chart an error message about the data.
            }

            return dashboardViewModel;
        }

        /// <summary>
        /// Get the dashboard macro chart data in the form of a list of DashbardMacroChartModel stored in a DashboardViewModel.
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <param name="userID"></param>
        /// <param name="startDate"></param>
        /// <returns>A list of Macro Chart Data staring with the most current date.</returns>
        public async Task<DashboardViewModel> GetDashboardMacroChartModels(DashboardViewModel dashboardViewModel, string userID, DateTime startDate)
        {
            if (dashboardViewModel is null)
            {
                dashboardViewModel = new DashboardViewModel();
            }

            int chartDays = (int)_globalProperties.Application["DashboardMacroChartDays"];
            var chartDate = new DateTime(startDate.Ticks);
            var tempDate = chartDate.AddDays(-1 * chartDays);

            dashboardViewModel.DashboardMacroChartModels = new List<Models.Charts.DashboardMacroChartModel>(chartDays);

            var dashboardMacroChartEntries = await _dbContext.DashboardMacroChartEnteries
                                                 .Where(e => e.UserId == userID && e.LogDate > tempDate)
                                                 .OrderByDescending(e => e.LogDate)
                                                 .Select(e => new { e.Id, e.LogDate, e.Fat, e.Carb, e.Protein })
                                                 .ToListAsync();

            if (dashboardMacroChartEntries is not null)
            {
                int i = 0;

                while (chartDate > tempDate)
                {
                    // Check if the current index in the list matches the current logDate.
                    if (i < dashboardMacroChartEntries.Count && dashboardMacroChartEntries[i].LogDate.Date == chartDate.Date)
                    {
                        dashboardViewModel.DashboardMacroChartModels.Add(new DashboardMacroChartModel()
                        {
                            Id = dashboardMacroChartEntries[i].Id,
                            Date = dashboardMacroChartEntries[i].LogDate.ToString("ddd, dd MMM"),
                            Fat = dashboardMacroChartEntries[i].Fat,
                            Carb = dashboardMacroChartEntries[i].Carb,
                            Protein = dashboardMacroChartEntries[i].Protein,
                        });

                        i++;
                    }
                    else
                    {
                        // No record for this day. Create default entry for this date.
                        dashboardViewModel.DashboardMacroChartModels.Add(new DashboardMacroChartModel()
                        {
                            Id = 0,
                            Date = chartDate.ToString("ddd, dd MMM"),
                            Fat = 0,
                            Carb = 0,
                            Protein = 0,
                        });
                    }

                    chartDate = chartDate.AddDays(-1);
                }
            }
            else
            {
                // No valid data from the database, make placeholder data.
                for (int i = 0; i < chartDays; i++)
                {
                    dashboardViewModel.DashboardMacroChartModels.Add(new DashboardMacroChartModel()
                    {
                        Id = 0,
                        Date = chartDate.DayOfWeek.ToString(),
                        Fat = 0,
                        Carb = 0,
                        Protein = 0,
                    });

                    chartDate = chartDate.AddDays(-1);
                }

                // TODO: Create a text entry to display on the chart an error message about the data.
            }

            return dashboardViewModel;
        }

        private List<DashboardCalorieChartEntry> CreateDashboardCalorieChartEntry(List<TransactionMacroLog> macroLogs, string? userID, DateTime startDate)
        {
            int dcceListSize = (int)_globalProperties.Application["DashboardCalorieChartDays"];

            List<DashboardCalorieChartEntry> dcceList = new List<DashboardCalorieChartEntry>(dcceListSize);

            for(int i=0; i < dcceListSize; i++)
            {
                dcceList.Add( new DashboardCalorieChartEntry());
            }

            var logDate = new DateTime(startDate.Ticks);
            int l = 0;

            // Iterate through each chart datapoint, and then search the macro log list for matching data.
            foreach (var dcce in dcceList)
            {
                // Set the current DashboardCaloriesChartEntry
                dcce.LogDate = logDate;
                dcce.UserId = userID;

                while (l < macroLogs.Count)
                {
                    // Check if the current index in the list matches the current logDate
                    if (logDate.Date == macroLogs[l].TimeStamp.Date)
                    {
                        // Add calories to the list
                        dcce.Calories += macroLogs[l].Calories;

                        l++;
                    }
                    else
                    {
                        break;
                    }
                }

                // Decrease the date one for the next object.
                logDate = logDate.AddDays(-1);
            }  

            return dcceList;
        }

        private List<DashboardMacroChartEntry> CreateDashboardMacroChartEntry(List<TransactionMacroLog> macroLogs, string? userID, DateTime startDate)
        {
            int dmceListSize = (int)_globalProperties.Application["DashboardMacroChartDays"];

            List<DashboardMacroChartEntry> dmceList = new List<DashboardMacroChartEntry>(dmceListSize);

            for (int i = 0; i < dmceListSize; i++)
            {
                dmceList.Add(new DashboardMacroChartEntry());
            }

            var logDate = new DateTime(startDate.Ticks);
            int l = 0;

            // Iterate through each chart datapoint, and then search the macro log list for matching data.
            foreach (var dmce in dmceList)
            {
                // Set the current DashboardCaloriesChartEntry
                dmce.LogDate = logDate;
                dmce.UserId = userID;

                float fat = 0;
                float carb = 0;
                float protein = 0;

                while (l < macroLogs.Count)
                {
                    // Check if the current index in the list matches the current logDate
                    if (logDate.Date == macroLogs[l].TimeStamp.Date)
                    {
                        // Sum Macros
                        fat += macroLogs[l].Fat;
                        carb += macroLogs[l].TotalCarb;
                        protein += macroLogs[l].Protein;

                        l++;
                    }
                    else
                    {
                        break;
                    }
                }

                dmce.Fat = (uint)MathF.Round(fat);
                dmce.Carb = (uint)MathF.Round(carb);
                dmce.Protein = (uint)MathF.Round(protein);

                // Decrease the date one for the next object.
                logDate = logDate.AddDays(-1);
            }

            return dmceList;
        }
    }    
}
