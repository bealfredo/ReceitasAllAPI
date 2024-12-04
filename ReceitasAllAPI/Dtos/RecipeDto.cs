using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class RecipeDto
    {

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Url]
        public string Image { get; set; }

        [Required]
        public Difficulty Difficulty { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        [Required]
        public int PreparationTimeInMinutes { get; set; }

        [Required]
        public string Rendimento { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateUpdated { get; set; }

        public string AccentColor { get; set; }

        public virtual List<StepDto> Steps { get; set; } = new List<StepDto>();

        public virtual List<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
    }
}
