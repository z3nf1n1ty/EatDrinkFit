// Project: EatDrinkFit.Web
// File: Models/ManualMacroLogViewModel.cs
// Origonially designed for ASP.NET Core 8.0

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// Copyright (C) 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor:

namespace EatDrinkFit.Web.Models
{
    public class ManualMacroLogViewModel
    {
        public DateTime TimeStamp { get; set; }

        public uint Calories { get; set; }

        public float Fat { get; set; }

        public float Cholesterol { get; set; }

        public float Sodium { get; set; }

        public float TotalCarb { get; set; }

        public float Fiber { get; set; }

        public float Sugar { get; set; }

        public float Protein { get; set; }

        public string? Note { get; set; }

        public float WaterAmmount { get; set; }

        public string? WaterUnit { get; set;}

        public string? WaterNote { get; set; }

        public DateTime WaterTimeStamp { get; set; }

        public float FluidAmmount { get; set; }

        public string? FluidUnit { get; set; }

        public string? FluidNote { get; set; }

        public DateTime FluidTimeStamp { get; set; }

        public string? ManualTZ { get; set; }

        public string? WaterTZ { get; set; }

        public string? FluidTZ { get; set; }
    }
}
