// Decompiled with JetBrains decompiler
// Type: ReceitasAllAPI.Controllers.FavoriteRecipesController
// Assembly: ReceitasAllAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9B75B2E0-CFCF-4AB8-8B7D-738B515A4450
// Assembly location: C:\Users\netto\source\repos\ReceitasAllAPI\ReceitasAllAPI\bin\Debug\net8.0\ReceitasAllAPI.dll
// XML documentation location: C:\Users\netto\source\repos\ReceitasAllAPI\ReceitasAllAPI\bin\Debug\net8.0\ReceitasAllAPI.xml

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ReceitasAllAPI.Dtos;
using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Persistence;
using System;
using System.Linq;
using System.Linq.Expressions;

#nullable enable
namespace ReceitasAllAPI.Controllers
{
  [Authorize]
  [Route("api/favoriterecipes")]
  [ApiController]
  public class FavoriteRecipesController : ControllerBase
  {
    private readonly ApplicationDbContext _context;

    public FavoriteRecipesController(ApplicationDbContext context) => this._context = context;

    /// <summary>
    /// Favorita uma receita específica para o autor autenticado.
    /// </summary>
    /// <param name="id">ID da receita a ser favoritada.</param>
    /// <returns>Status indicando o resultado da operação.</returns>
    /// <response code="200">A receita foi favoritada com sucesso.</response>
    /// <response code="400">A requisição é inválida. Possíveis razões incluem:
    /// <list type="bullet">
    /// <item><description>A receita é privada e não pode ser favoritada.</description></item>
    /// </list>
    /// </response>
    /// <response code="404">O usuário ou a receita não foram encontrados.</response>
    /// <response code="409">A receita já foi favoritada anteriormente.</response>
    /// <response code="500">Erro interno ao processar a solicitação.</response>
    /// <remarks>
    /// Este endpoint permite que um usuário autenticado favorite uma receita.
    /// Apenas receitas públicas podem ser favoritas e um mesmo usuário não pode favoritar a mesma receita mais de uma vez.
    /// </remarks>
    [HttpPost("{id}")]
    public IActionResult Favorite(int id)
    {
      string userName = this.User?.Identity?.Name;
      Author author = this._context.Authors.SingleOrDefault<Author>((Expression<Func<Author, bool>>) (a => a.UserName == userName));
      if (author == null)
        return (IActionResult) this.NotFound((object) "Usuário não encontrado.");
      using (IDbContextTransaction contextTransaction = this._context.Database.BeginTransaction())
      {
        try
        {
          Recipe recipe = this._context.Recipes.Include<Recipe, Author>((Expression<Func<Recipe, Author>>) (r => r.Author)).FirstOrDefault<Recipe>((Expression<Func<Recipe, bool>>) (r => r.ID == id));
          if (recipe == null)
            return (IActionResult) this.NotFound((object) "Receita não encontrada.");
          if (recipe.IsPrivate)
            return (IActionResult) this.BadRequest((object) "Não é possível favoritar uma receita privada.");
          if (this._context.FavoriteRecipes.Any<FavoriteRecipe>((Expression<Func<FavoriteRecipe, bool>>) (fr => fr.AuthorId == author.ID && fr.RecipeId == recipe.ID)))
            return (IActionResult) this.Conflict((object) "Receita já favoritada.");
          this._context.FavoriteRecipes.Add(new FavoriteRecipe()
          {
            AuthorId = author.ID,
            RecipeId = recipe.ID,
            DateAdded = DateTime.Now
          });
          this._context.SaveChanges();
          contextTransaction.Commit();
          return (IActionResult) this.Ok();
        }
        catch (Exception ex)
        {
          contextTransaction.Rollback();
          return (IActionResult) this.StatusCode(400, (object) "Erro ao favoritar a receita.");
        }
      }
    }

    [HttpDelete("{id}")]
    public IActionResult Unfavorite(int id)
    {
      using (IDbContextTransaction contextTransaction = this._context.Database.BeginTransaction())
      {
        try
        {
          string userName = this.User?.Identity?.Name;
          Author author = this._context.Authors.SingleOrDefault<Author>((Expression<Func<Author, bool>>) (a => a.UserName == userName));
          if (author == null)
            return (IActionResult) this.NotFound((object) "Usuário não encontrado.");
          FavoriteRecipe entity = this._context.FavoriteRecipes.Include<FavoriteRecipe, Recipe>((Expression<Func<FavoriteRecipe, Recipe>>) (fr => fr.Recipe)).FirstOrDefault<FavoriteRecipe>((Expression<Func<FavoriteRecipe, bool>>) (fr => fr.AuthorId == author.ID && fr.RecipeId == id));
          if (entity == null)
            return (IActionResult) this.BadRequest((object) "A receita não existe ou você ainda não favoritou a receita.");
          this._context.FavoriteRecipes.Remove(entity);
          this._context.SaveChanges();
          contextTransaction.Commit();
          return (IActionResult) this.NoContent();
        }
        catch (Exception ex)
        {
          contextTransaction.Rollback();
          return (IActionResult) this.StatusCode(400, (object) "Erro ao desfavoritar a receita.");
        }
      }
    }

    [HttpGet]
    public IActionResult GetMyFavorites()
    {
      string userName = this.User?.Identity?.Name;
      Author author = this._context.Authors.SingleOrDefault<Author>((Expression<Func<Author, bool>>) (a => a.UserName == userName));
      if (author == null)
        return (IActionResult) this.NotFound((object) "Usuário não encontrado.");
      return (IActionResult) this.Ok((object) this._context.FavoriteRecipes.Include<FavoriteRecipe, Recipe>((Expression<Func<FavoriteRecipe, Recipe>>) (fr => fr.Recipe)).Include<FavoriteRecipe, Author>((Expression<Func<FavoriteRecipe, Author>>) (fr => fr.Recipe.Author)).Where<FavoriteRecipe>((Expression<Func<FavoriteRecipe, bool>>) (fr => fr.AuthorId == author.ID && !fr.Recipe.IsPrivate)).Select<FavoriteRecipe, RecipeResponseDto>((Expression<Func<FavoriteRecipe, RecipeResponseDto>>) (fr => RecipeResponseDto.FromEntity(fr.Recipe))).ToList<RecipeResponseDto>());
    }
  }
}
