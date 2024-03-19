using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EatDrinkFit.Web.Models.Entities
{
	public class MacroLog
	{
		[Key]
        public uint Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

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
    }
}
