// Project: EatDrinkFit.Web
// File: Models/Charts/DashboardPercentCalChartModel.cs
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
    public class DashboardPercentCalChartModel
    {
        public uint Id { get; set; }

        public string Date { get; set; } = string.Empty;

        public uint PercentOther { get; set; }

        public uint PercentFat { get; set; }

        public uint PercentCarb { get; set; }

        public uint PercentProtein { get; set; }
    }
}
