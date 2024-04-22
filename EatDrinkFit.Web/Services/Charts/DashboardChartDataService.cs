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

        public Task<DashboardViewModel> GetDashboardMicroChartModels(DashboardViewModel dashboardViewModel, string userID, DateTime startDate);

        public Task<DashboardViewModel> GetDashboardPercentCalChartModels(DashboardViewModel dashboardViewModel, string userID, DateTime startDate);
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

            // Process the micro data into the micro chart data.
            List<DashboardMicroChartEntry> dmiaceList = CreateDashboardMicroChartEntry(macroLogs, userID, startDate);

            // Store the macro chart data.
            foreach (var r in dmiaceList)
            {
                if (r is not null)
                {
                    if (await _dbContext.DashboardMicroChartEnteries.AnyAsync(e => e == r))
                    {
                        _dbContext.DashboardMicroChartEnteries.Update(r).Property(x => x.Id).IsModified = false;
                    }
                    else
                    {
                        //await _dbContext.DashboardMacroChartEnteries.AddAsync(r);
                        _dbContext.DashboardMicroChartEnteries.Add(r);
                        // Add should be local until the savechanges, since the identity does not need to be used.
                        // Therfore the local add should be faster by removing the additional task related job overhead that is not providing benifit.
                        // Ref: https://medium.com/@iamprovidence/addasync-in-ef-is-pure-evil-d31231d8f04e
                    }
                }
            }

            await _dbContext.SaveChangesAsync();

            // Process the macro data into the percent calorie chart data.
            List<DashboardPercentCalChartEntry> dpcceList = CreateDashboardPercentCaloriesChartEntry(macroLogs, userID, startDate);

            // Store the macro chart data.
            foreach (var r in dpcceList)
            {
                if (r is not null)
                {
                    if (await _dbContext.DashboardPercentCalChartEntry.AnyAsync(e => e == r))
                    {
                        _dbContext.DashboardPercentCalChartEntry.Update(r).Property(x => x.Id).IsModified = false;
                    }
                    else
                    {
                        //await _dbContext.DashboardMacroChartEnteries.AddAsync(r);
                        _dbContext.DashboardPercentCalChartEntry.Add(r);
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

        /// <summary>
        /// Get the dashboard micro chart data in the form of a list of DashbardMicroChartModel stored in a DashboardViewModel.
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <param name="userID"></param>
        /// <param name="startDate"></param>
        /// <returns>A list of Micro Chart Data staring with the most current date.</returns>
        public async Task<DashboardViewModel> GetDashboardMicroChartModels(DashboardViewModel dashboardViewModel, string userID, DateTime startDate)
        {
            if (dashboardViewModel is null)
            {
                dashboardViewModel = new DashboardViewModel();
            }

            int chartDays = (int)_globalProperties.Application["DashboardMicroChartDays"];
            var chartDate = new DateTime(startDate.Ticks);
            var tempDate = chartDate.AddDays(-1 * chartDays);

            dashboardViewModel.DashboardMicroChartModels = new List<Models.Charts.DashboardMicroChartModel>(chartDays);

            var dashboardMicroChartEntries = await _dbContext.DashboardMicroChartEnteries
                                                 .Where(e => e.UserId == userID && e.LogDate > tempDate)
                                                 .OrderByDescending(e => e.LogDate)
                                                 .Select(e => new { e.Id, e.LogDate, e.Cholesterol, e.Sodium, e.Fiber, e.Sugar })
                                                 .ToListAsync();

            if (dashboardMicroChartEntries is not null)
            {
                int i = 0;

                while (chartDate > tempDate)
                {
                    // Check if the current index in the list matches the current logDate.
                    if (i < dashboardMicroChartEntries.Count && dashboardMicroChartEntries[i].LogDate.Date == chartDate.Date)
                    {
                        dashboardViewModel.DashboardMicroChartModels.Add(new DashboardMicroChartModel()
                        {
                            Id = dashboardMicroChartEntries[i].Id,
                            Date = dashboardMicroChartEntries[i].LogDate.ToString("ddd, dd MMM"),
                            Cholesterol = dashboardMicroChartEntries[i].Cholesterol,
                            Sodium = dashboardMicroChartEntries[i].Sodium,
                            Fiber = dashboardMicroChartEntries[i].Fiber,
                            Sugar = dashboardMicroChartEntries[i].Sugar,
                        });

                        i++;
                    }
                    else
                    {
                        // No record for this day. Create default entry for this date.
                        dashboardViewModel.DashboardMicroChartModels.Add(new DashboardMicroChartModel()
                        {
                            Id = 0,
                            Date = chartDate.ToString("ddd, dd MMM"),
                            Cholesterol = 0,
                            Sodium = 0,
                            Fiber = 0,
                            Sugar = 0,
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
                    dashboardViewModel.DashboardMicroChartModels.Add(new DashboardMicroChartModel()
                    {
                        Id = 0,
                        Date = chartDate.DayOfWeek.ToString(),
                        Cholesterol = 0,
                        Sodium = 0,
                        Fiber = 0,
                        Sugar = 0,
                    });

                    chartDate = chartDate.AddDays(-1);
                }

                // TODO: Create a text entry to display on the chart an error message about the data.
            }

            return dashboardViewModel;
        }

        /// <summary>
        /// Get the dashboard percent calorie chart data in the form of a list of DashboardPercentCalChartModel stored in a DashboardViewModel.
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <param name="userID"></param>
        /// <param name="startDate"></param>
        /// <returns>A list of Percent Calorie Chart Data staring with the most current date.</returns>
        public async Task<DashboardViewModel> GetDashboardPercentCalChartModels(DashboardViewModel dashboardViewModel, string userID, DateTime startDate)
        {
            if (dashboardViewModel is null)
            {
                dashboardViewModel = new DashboardViewModel();
            }

            int chartDays = (int)_globalProperties.Application["DashboardPercentCaloriesChartDays"];
            var chartDate = new DateTime(startDate.Ticks);
            var tempDate = chartDate.AddDays(-1 * chartDays);

            dashboardViewModel.DashboardPercentCalChartModel = new List<Models.Charts.DashboardPercentCalChartModel>(chartDays);

            var dashboardPercentCalChartEntries = await _dbContext.DashboardPercentCalChartEntry
                                                 .Where(e => e.UserId == userID && e.LogDate > tempDate)
                                                 .OrderByDescending(e => e.LogDate)
                                                 .Select(e => new { e.Id, e.LogDate, e.PercentOther, e.PercentFat, e.PercentCarb, e.PercentProtein })
                                                 .ToListAsync();

            if (dashboardPercentCalChartEntries is not null)
            {
                int i = 0;

                while (chartDate > tempDate)
                {
                    // Check if the current index in the list matches the current logDate.
                    if (i < dashboardPercentCalChartEntries.Count && dashboardPercentCalChartEntries[i].LogDate.Date == chartDate.Date)
                    {
                        dashboardViewModel.DashboardPercentCalChartModel.Add(new DashboardPercentCalChartModel()
                        {
                            Id = dashboardPercentCalChartEntries[i].Id,
                            Date = dashboardPercentCalChartEntries[i].LogDate.ToString("ddd, dd MMM"),
                            PercentOther = dashboardPercentCalChartEntries[i].PercentOther,
                            PercentFat = dashboardPercentCalChartEntries[i].PercentFat,
                            PercentCarb = dashboardPercentCalChartEntries[i].PercentCarb,
                            PercentProtein = dashboardPercentCalChartEntries[i].PercentProtein,
                        });

                        i++;
                    }
                    else
                    {
                        // No record for this day. Create default entry for this date.
                        dashboardViewModel.DashboardPercentCalChartModel.Add(new DashboardPercentCalChartModel()
                        {
                            Id = 0,
                            Date = chartDate.ToString("ddd, dd MMM"),
                            PercentOther = 0,
                            PercentFat = 0,
                            PercentCarb = 0,
                            PercentProtein = 0,
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
                    dashboardViewModel.DashboardPercentCalChartModel.Add(new DashboardPercentCalChartModel()
                    {
                        Id = 0,
                        Date = chartDate.DayOfWeek.ToString(),
                        PercentOther = 0,
                        PercentFat = 0,
                        PercentCarb = 0,
                        PercentProtein = 0,
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

        private List<DashboardMicroChartEntry> CreateDashboardMicroChartEntry(List<TransactionMacroLog> macroLogs, string? userID, DateTime startDate)
        {
            int dmceListSize = (int)_globalProperties.Application["DashboardMicroChartDays"];

            List<DashboardMicroChartEntry> dmceList = new List<DashboardMicroChartEntry>(dmceListSize);

            for (int i = 0; i < dmceListSize; i++)
            {
                dmceList.Add(new DashboardMicroChartEntry());
            }

            var logDate = new DateTime(startDate.Ticks);
            int l = 0;

            // Iterate through each chart datapoint, and then search the macro log list for matching data.
            foreach (var dmce in dmceList)
            {
                // Set the current DashboardCaloriesChartEntry
                dmce.LogDate = logDate;
                dmce.UserId = userID;

                float cholesterol = 0;
                float sodium = 0;
                float fiber = 0;
                float sugar = 0;

                while (l < macroLogs.Count)
                {
                    // Check if the current index in the list matches the current logDate
                    if (logDate.Date == macroLogs[l].TimeStamp.Date)
                    {
                        // Sum Macros
                        cholesterol += macroLogs[l].Cholesterol;
                        sodium += macroLogs[l].Sodium;
                        fiber += macroLogs[l].Fiber;
                        sugar += macroLogs[l].Protein;

                        l++;
                    }
                    else
                    {
                        break;
                    }
                }

                dmce.Cholesterol = (uint)MathF.Round(cholesterol);
                dmce.Sodium = (uint)MathF.Round(sodium);
                dmce.Fiber = (uint)MathF.Round(fiber);
                dmce.Sugar = (uint)MathF.Round(sugar);

                // Decrease the date one for the next object.
                logDate = logDate.AddDays(-1);
            }

            return dmceList;
        }

        private List<DashboardPercentCalChartEntry> CreateDashboardPercentCaloriesChartEntry(List<TransactionMacroLog> macroLogs, string? userID, DateTime startDate)
        {
            int dpcceListSize = (int)_globalProperties.Application["DashboardPercentCaloriesChartDays"];

            List<DashboardPercentCalChartEntry> dpcceList = new List<DashboardPercentCalChartEntry>(dpcceListSize);

            for (int i = 0; i < dpcceListSize; i++)
            {
                dpcceList.Add(new DashboardPercentCalChartEntry());
            }

            var logDate = new DateTime(startDate.Ticks);
            int l = 0;

            // Iterate through each chart datapoint, and then search the macro log list for matching data.
            foreach (var dpcce in dpcceList)
            {
                // Set the current DashboardCaloriesChartEntry
                dpcce.LogDate = logDate;
                dpcce.UserId = userID;

                float fat = 0;
                float carb = 0;
                float protein = 0;
                float cal = 0;

                while (l < macroLogs.Count)
                {
                    // Check if the current index in the list matches the current logDate
                    if (logDate.Date == macroLogs[l].TimeStamp.Date)
                    {
                        // Sum Macros
                        fat += macroLogs[l].Fat;
                        carb += macroLogs[l].TotalCarb;
                        protein += macroLogs[l].Protein;
                        cal += macroLogs[l].Calories;

                        l++;
                    }
                    else
                    {
                        break;
                    }
                }

                uint pFat = (uint)MathF.Floor(( fat * 9 ) / cal * 100);
                uint pCarb = (uint)MathF.Floor(( carb * 4 ) / cal * 100);
                uint pProtein = (uint)MathF.Floor(( protein * 4 ) / cal * 100);
                uint pOther = 0;

                uint sum = pFat + pCarb + pProtein;

                if (sum == 100)
                {
                    // macros equal 100% no other to calculate or adjust.
                }
                else if (sum < 100)
                {
                    // macros are less than 100% and need to calculate other.
                    pOther = 100 - sum;
                }
                else
                {
                    // macros are more than 100% need to adjust for errors.

                    pFat = (uint)MathF.Floor(pFat / sum * 100);
                    pCarb = (uint)MathF.Floor(pCarb / sum * 100);
                    pProtein = (uint)MathF.Floor(pProtein / sum * 100);

                    pOther = 100 - pFat - pCarb - pProtein;
                    
                }                

                dpcce.PercentFat = pFat;
                dpcce.PercentCarb = pCarb;
                dpcce.PercentProtein = pProtein;
                dpcce.PercentOther = pOther;

                // Decrease the date one for the next object.
                logDate = logDate.AddDays(-1);
            }

            return dpcceList;
        }
    }    
}
