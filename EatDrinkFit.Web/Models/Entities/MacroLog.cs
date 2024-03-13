using System.ComponentModel.DataAnnotations;

namespace EatDrinkFit.Web.Models.Entities
{
	public class MacroLog
	{
		[Key]
        public uint Id { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        public int Calories { get; set; }

        public int Fat { get; set; }

        public int Cholesterol { get; set; }

        public int TotalCarb { get; set; }

        public int Fiber { get; set; }

        public int Sugar { get; set; }

        public int Protein { get; set; }

        public string? Note { get; set; }
    }
}
