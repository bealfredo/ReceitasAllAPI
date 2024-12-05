using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Entities
{
    public class RecipeCookbook
    {
        public RecipeCookbook()
        {

        }

        public int ID { get; set; }
        [Required]
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        [Required]
        public int CookbookId { get; set; }
        public Cookbook Cookbook { get; set; }
        [Required]
        [Display(Name = "Prioridade de Exibição")]
        public int Order { get; set; }
        [Required]
        [Display(Name = "Data de adição")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }
    }
}
