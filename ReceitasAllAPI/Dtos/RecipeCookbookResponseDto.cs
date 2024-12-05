using ReceitasAllAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class RecipeCookbookResponseDto
    {
        public int ID { get; set; }
        public RecipeResponseDto Recipe { get; set; }
        public int Order { get; set; }
        public DateTime DateAdded { get; set; }

        public static RecipeCookbookResponseDto FromEntity(RecipeCookbook recipeCookbook)
        {
            if (recipeCookbook == null)
            {
                return null;
            }

            return new RecipeCookbookResponseDto
            {
                ID = recipeCookbook.ID,
                Recipe = RecipeResponseDto.FromEntity(recipeCookbook.Recipe),
                Order = recipeCookbook.Order,
                DateAdded = recipeCookbook.DateAdded
            };
        } 

    }
}