using ReceitasAllAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class RecipeCookbookDto
    {
        [Required]
        public int RecipeId { get; set; }
        [Required]
        public int Order { get; set; }
    }
}
