// Project: EatDrinkFit.Web
// File: Models/Entities/MacroLog.cs
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

namespace EatDrinkFit.Web.Models.Entities
{
	public class MacroLog
	{
		[Key]
        public uint Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public string? Timezone { get; set; }

        [Required]
        public uint Calories { get; set; }

        public float Fat { get; set; }

        public float Cholesterol { get; set; }

        public float Sodium { get; set; }

        public float TotalCarb { get; set; }

        public float Fiber { get; set; }

        public float Sugar { get; set; }

        public float Protein { get; set; }

        public string? Note { get; set; }

        public MacroLogSource Source { get; set; }

        public bool FromFavorites { get; set; }

        public bool IsFavorite { get; set; }
    }

    public enum MacroLogSource
    {
        Undefined = 0,
        Meal = 1,
        Component = 2,
        Ingredient = 3,
        Manual = 4,
    }

}
