using ReceitasAllAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Dtos
{
    public class StepDto
    {
        [Required]
        [Display(Name = "Prioridade de Exibição")]
        public int Order { get; set; }
        [Required]
        [Display(Name = "Passo")]
        [StringLength(500, MinimumLength = 1)]
        public string Value { get; set; }

    }
}
