// Project: EatDrinkFit.Web
// File: Models/Entities/Charts/DashboardMicroChart.cs
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
using System.ComponentModel.DataAnnotations.Schema;

namespace EatDrinkFit.Web.Models.Entities.Charts
{
    [PrimaryKey(nameof(UserId), nameof(LogDate))]
    [Index(nameof(Id), IsUnique = true)]
    public class DashboardMicroChartEntry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public DateTime LogDate { get; set; }

        [Required]
        public uint Cholesterol { get; set; }

        [Required]
        public uint Sodium { get; set; }

        [Required]
        public uint Fiber { get; set; }

        [Required]
        public uint Sugar { get; set; }
    }
}
