using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Entities
{
    public class Ingredient
    {
        public Ingredient()
        {

        }
        public int ID { get; set; }
        [Required]
        [Display(Name = "Prioridade de Exibição")]
        public int Order { get; set; }
        [Required]
        [Display(Name = "Ingrediente")]
        [StringLength(500, MinimumLength = 1)]
        public string Value { get; set; }
        [Required]
        public int RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; }
    }
}
