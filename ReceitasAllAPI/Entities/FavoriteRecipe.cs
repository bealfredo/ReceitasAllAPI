using System;
using System.ComponentModel.DataAnnotations;

#nullable enable
namespace ReceitasAllAPI.Entities
{
    public class FavoriteRecipe
    {
        public int ID { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }

        [Required]
        public int RecipeId { get; set; }

        public virtual Recipe Recipe { get; set; }

        [Required]
        [Display(Name = "Data favoritado")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }
    }
}
