using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class CookbookDto
    {

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Url]
        public string Image { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

        public string AccentColor { get; set; }

        public virtual List<RecipeCookbookDto> Recipes { get; set; } = new List<RecipeCookbookDto>();
    }
}
