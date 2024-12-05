using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ReceitasAllAPI.Dtos
{
    public class CookbookResponseDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string AccentColor { get; set; }
        public virtual AuthorResponseDto Author { get; set; }
        public virtual List<RecipeCookbookResponseDto> RecipeCookbook { get; set; } = new List<RecipeCookbookResponseDto>();

        // Function to convert an entity to a DTO
        public static CookbookResponseDto FromEntity(Cookbook cookbook)
        {
            if (cookbook == null)
            {
                return null;
            }

            return new CookbookResponseDto
            {
                ID = cookbook.ID,
                Title = cookbook.Title,
                Description = cookbook.Description,
                Image = cookbook.Image,
                IsPrivate = cookbook.IsPrivate,
                DateAdded = cookbook.DateAdded,
                DateUpdated = cookbook.DateUpdated,
                AccentColor = cookbook.AccentColor,
                Author = AuthorResponseDto.FromEntity(cookbook.Author),
                RecipeCookbook = cookbook.RecipeCookbooks.Select(rc => RecipeCookbookResponseDto.FromEntity(rc)).ToList()
            };
        }
    }
}
