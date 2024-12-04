using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class AuthorUpdateDto
    {
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
    }
}
