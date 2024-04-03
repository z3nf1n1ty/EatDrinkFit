// Project: EatDrinkFit.Web
// File: Models/Charts/DashboardMicroChartModel.cs
// Origonially designed for ASP.NET Core 8.0

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// Copyright (C) 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor:

using System.ComponentModel.DataAnnotations.Schema;

namespace EatDrinkFit.Web.Models.Charts
{
    public class DashboardMicroChartModel
    {
        public uint Id { get; set; }

        public string Date { get; set; } = string.Empty;

        public uint Cholesterol { get; set; }

        public uint Sodium { get; set; }

        public uint Fiber { get; set; }

        public uint Sugar { get; set; }
    }
}
