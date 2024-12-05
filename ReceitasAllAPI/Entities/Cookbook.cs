using System.ComponentModel.DataAnnotations;

namespace ReceitasAllAPI.Entities
{
    public class Cookbook
    {
        public Cookbook()
        {

        }
        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "Título")]
        public string Title { get; set; }

        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Url]
        [Display(Name = "Imagem (URL)")]
        public string Image { get; set; }

        [Required]
        [Display(Name = "Livro privado")]
        public bool IsPrivate { get; set; }

        [Required]
        [Display(Name = "Data de criação")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }

        [Required]
        [Display(Name = "Data de modificação")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateUpdated { get; set; }

        [Display(Name = "Cor de destaque (Cor CSS)")]
        public string AccentColor { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public virtual Author Author { get; set; }

        public virtual List<RecipeCookbook> RecipeCookbooks { get; set; } = new List<RecipeCookbook>();
    }
}
