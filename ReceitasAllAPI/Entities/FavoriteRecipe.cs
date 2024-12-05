// Decompiled with JetBrains decompiler
// Type: ReceitasAllAPI.Entities.FavoriteRecipe
// Assembly: ReceitasAllAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9B75B2E0-CFCF-4AB8-8B7D-738B515A4450
// Assembly location: C:\Users\netto\source\repos\ReceitasAllAPI\ReceitasAllAPI\bin\Debug\net8.0\ReceitasAllAPI.dll
// XML documentation location: C:\Users\netto\source\repos\ReceitasAllAPI\ReceitasAllAPI\bin\Debug\net8.0\ReceitasAllAPI.xml

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
