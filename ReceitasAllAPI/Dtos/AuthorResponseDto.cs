using ReceitasAllAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class AuthorResponseDto
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public bool Admin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nacionality { get; set; }
        public string Image { get; set; }
        public string Bibliography { get; set; }
        public string Pseudonym { get; set; }
        public string EmailContact { get; set; }

        // Function to convert an entity to a DTO
        public static AuthorResponseDto FromEntity(Author author)
        {
            if (author == null)
            {
                return null;
            }

            return new AuthorResponseDto
            {
                ID = author.ID,
                UserName = author.UserName,
                Admin = author.Admin,
                FirstName = author.FirstName,
                LastName = author.LastName,
                Nacionality = author.Nacionality,
                Image = author.Image,
                Bibliography = author.Bibliography,
                Pseudonym = author.Pseudonym,
                EmailContact = author.EmailContact
            };
        }
    }
}
