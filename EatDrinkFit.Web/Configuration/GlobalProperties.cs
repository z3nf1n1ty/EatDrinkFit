// Project: EatDrinkFit.Web
// File: GlobalProperties.cs
// Origonially designed for ASP.NET Core 8.0

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// Copyright (C) 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor:

using System.Collections.Concurrent;

namespace EatDrinkFit.Web.Configuration
{
    public interface IGlobalProperties
    {
        ConcurrentDictionary<string, object> Application { get; }
    }

    /// <summary>
    /// Privoid access to global application data via a thread save dictionary and dependency injections.
    /// </summary>
    public sealed class GlobalProperties : IGlobalProperties
    {
        public ConcurrentDictionary<string, object> Application { get; } = new ConcurrentDictionary<string, object>()
        {
            //["Setting"] = true,

            // Database Settings
            ["DatabaseTimezoneIANA"] = true,

            // Chart Settings
            ["DashboardCalorieChartDays"] = 14,
            ["DashboardMacroChartDays"] = 7,
            ["DashboardMicroChartDays"] = 7,
            ["DashboardPercentCaloriesChartDays"] = 1,
        };

        //public static IServiceCollection AddGlobalProperties(IServiceCollection services)
        //{
        //    services.AddSingleton<GlobalProperties>();

        //    return services;
        //}
    }
}
