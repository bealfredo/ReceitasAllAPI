using ReceitasAllAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class IngredientDto
    {
        [Required]
        [Display(Name = "Prioridade de Exibição")]
        public int Order { get; set; }
        [Required]
        [Display(Name = "Ingrediente")]
        [StringLength(500, MinimumLength = 1)]
        public string Value { get; set; }
    }
}
