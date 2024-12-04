using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class RecipeResponseDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Difficulty Difficulty { get; set; }
        public bool IsPrivate { get; set; }
        public int PreparationTimeInMinutes { get; set; }
        public string Rendimento { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string AccentColor { get; set; }
        public AuthorResponseDto Author { get; set; }
        public List<StepResponseDto> Steps { get; set; }
        public List<IngredientResponseDto> Ingredients { get; set; }

        // Function to convert an entity to a DTO
        public static RecipeResponseDto FromEntity(Recipe recipe)
        {
            if (recipe == null)
            {
                return null;
            }

            return new RecipeResponseDto
            {
                ID = recipe.ID,
                Title = recipe.Title,
                Description = recipe.Description,
                Image = recipe.Image,
                Difficulty = recipe.Difficulty,
                IsPrivate = recipe.IsPrivate,
                PreparationTimeInMinutes = recipe.PreparationTimeInMinutes,
                Rendimento = recipe.Rendimento,
                DateAdded = recipe.DateAdded,
                DateUpdated = recipe.DateUpdated,
                AccentColor = recipe.AccentColor,
                Author = AuthorResponseDto.FromEntity(recipe.Author),
                Steps = recipe.Steps.Select(StepResponseDto.FromEntity).ToList(),
                Ingredients = recipe.Ingredients.Select(IngredientResponseDto.FromEntity).ToList()
            };
        }
    }
}
