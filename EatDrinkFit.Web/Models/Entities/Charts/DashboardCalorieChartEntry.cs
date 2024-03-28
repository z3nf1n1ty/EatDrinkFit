// Project: EatDrinkFit.Web
// File: Models/Entities/Charts/DashboardCalorieChart.cs
// Origonially designed for ASP.NET Core 8.0

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// Copyright (C) 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor:

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace EatDrinkFit.Web.Models.Entities.Charts
{
    [PrimaryKey(nameof(UserId), nameof(LogDate))]
    [Index(nameof(Id), IsUnique = true)]
    public class DashboardCalorieChartEntry
    {
        public uint Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public DateTime LogDate { get; set; }

        [Required]
        public uint Calories { get; set; }
    }
}
