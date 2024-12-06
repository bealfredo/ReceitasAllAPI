using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class AuthorUpdatePasswordDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string NewPassword { get; set; }

    }
}
