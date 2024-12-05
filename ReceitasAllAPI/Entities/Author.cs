using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Entities
{
    public class Author
    {
        public Author()
        {
            Recipes = new List<Recipe>();
        }

        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string PasswordHash { get; set; }

        [Required]
        public bool Admin { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }

        [StringLength(50, MinimumLength = 1)]
        public string Nacionality { get; set; }

        public string Image { get; set; }

        [StringLength(500, MinimumLength = 1)]
        public string Bibliography { get; set; }

        [StringLength(50, MinimumLength = 1)]
        public string Pseudonym { get; set; }

        [EmailAddress]
        public string EmailContact { get; set; }

        //[Required]
        //public string UserId { get; set; }

        public virtual List<Recipe> Recipes { get; set; } = new List<Recipe>();

        public virtual List<Cookbook> Cookbooks { get; set; } = new List<Cookbook>();

        public virtual List<FavoriteRecipe> FavoriteRecipes { get; set; } = new List<FavoriteRecipe>();



        //public void Update(Author author)
        //{
        //    FirstName = author.FirstName;
        //    LastName = author.LastName;
        //    Nacionality = author.Nacionality;
        //    Image = author.Image;
        //    Bibliography = author.Bibliography;
        //    Pseudonym = author.Pseudonym;
        //    EmailContact = author.EmailContact;
        //}

        //public void Delete()
        //{
        //    FirstName = null;
        //    LastName = null;
        //    Nacionality = null;
        //    Image = null;
        //    Bibliography = null;
        //    Pseudonym = null;
        //    EmailContact = null;
        //}
    }
}
