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

namespace ReceitasAllAPI.Controllers
{
    /// <summary>
    /// Controlador de operações relacionadas a favoritar receitas.
    /// </summary>
    [Authorize]
    [Route("api/favoriterecipes")]
    [ApiController]
    public class FavoriteRecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Este método é o construtor da classe.
        /// </summary>
        /// <param name="context"></param>
        public FavoriteRecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("{id}")]
        public IActionResult Favorite(int id)
        {
            var userName = User?.Identity?.Name;
            var author = _context.Authors.SingleOrDefault(a => a.UserName == userName);
            if (author == null)
                return NotFound("Usuário não encontrado.");

            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var recipe = _context.Recipes.Include(r => r.Author).FirstOrDefault(r => r.ID == id);
                    if (recipe == null)
                        return NotFound("Receita não encontrada.");
                    if (recipe.IsPrivate)
                        return BadRequest("Não é possível favoritar uma receita privada.");
                    if (_context.FavoriteRecipes.Any(fr => fr.AuthorId == author.ID && fr.RecipeId == recipe.ID))
                        return Conflict("Receita já favoritada.");

                    _context.FavoriteRecipes.Add(new FavoriteRecipe
                    {
                        AuthorId = author.ID,
                        RecipeId = recipe.ID,
                        DateAdded = DateTime.Now
                    });
                    _context.SaveChanges();
                    contextTransaction.Commit();
                    return Ok();
                }
                catch (Exception)
                {
                    contextTransaction.Rollback();
                    return StatusCode(400, "Erro ao favoritar a receita.");
                }
            }
        }

        /// <summary>
        /// Desfavorita uma receita específica para o autor autenticado.
        /// </summary>
        /// <param name="id">ID da receita a ser desfavoritada.</param>
        /// <returns>Status indicando o resultado da operação.</returns>
        /// <response code="204">A receita foi desfavoritada com sucesso.</response>
        /// <response code="400">A requisição é inválida. Possíveis razões incluem:
        /// A receita não foi encontrada ou não está nos favoritos do usuário.
        /// </response>
        /// <response code="404">O usuário não foi encontrado.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint permite que um usuário autenticado remova uma receita dos seus favoritos.
        /// Apenas receitas previamente favoritada pelo usuário podem ser removidas.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public IActionResult Unfavorite(int id)
        {
            using (IDbContextTransaction contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var userName = User?.Identity?.Name;
                    var author = _context.Authors.SingleOrDefault(a => a.UserName == userName);
                    if (author == null)
                        return NotFound("Usuário não encontrado.");

                    var entity = _context.FavoriteRecipes.Include(fr => fr.Recipe).FirstOrDefault(fr => fr.AuthorId == author.ID && fr.RecipeId == id);
                    if (entity == null)
                        return BadRequest("A receita não existe ou você ainda não favoritou a receita.");

                    _context.FavoriteRecipes.Remove(entity);
                    _context.SaveChanges();
                    contextTransaction.Commit();
                    return NoContent();
                }
                catch (Exception)
                {
                    contextTransaction.Rollback();
                    return StatusCode(400, "Erro ao desfavoritar a receita.");
                }
            }
        }

        /// <summary>
        /// Obtém a lista de receitas favoritas do autor autenticado.
        /// </summary>
        /// <returns>Uma lista de receitas favoritas públicas ao autor autenticado.</returns>
        /// <response code="200">A lista de receitas favoritas foi retornada com sucesso.</response>
        /// <response code="404">O usuário não foi encontrado.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint retorna todas as receitas favoritas do usuário autenticado que não são privadas.
        /// Cada receita é acompanhada das informações do autor da receita.
        /// 
        /// Exemplo de retorno:
        /// 
        ///
        ///     [
        ///         {
        ///             "id": 101,
        ///             "title": "Bolo de Chocolate",
        ///             "description": "Como não pensar em um bolo de chocolate com morango e não salivar? O bolo sensação é o doce que conquista o paladar de muita gente, já que o morango quebra o gosto adocicado do chocolate.",
        ///             "image": "",
        ///             "difficulty": 1,
        ///             "isPrivate": false,
        ///             "preparationTimeInMinutes": 45,
        ///             "rendimento": "8 porções",
        ///             "dateAdded": "2024-10-03T08:15:33",
        ///             "dateUpdated": "2024-10-04T09:12:33",
        ///             "accentColor": "#8B4513",
        ///             "author": {
        ///                 "id": 2,
        ///                 "userName": "mary",
        ///                 "admin": false,
        ///                 "firstName": "Maria",
        ///                 "lastName": "da Silva",
        ///                 "nacionality": "Brasileira",
        ///                 "image": "",
        ///                 "bibliography": "Uma mulher que adora cozinhar e compartilhar suas receitas",
        ///                 "pseudonym": "mary",
        ///                 "emailContact": "maria@gmail.com"
        ///             },
        ///             "steps": [
        ///                 {
        ///                     "id": 1,
        ///                     "order": 1,
        ///                     "value": "Em uma batedeira, bata as claras em neve."
        ///                 }
        ///             ],
        ///             "ingredients": [
        ///                 {
        ///                     "id": 1,
        ///                     "order": 1,
        ///                     "value": "2 xícaras de farinha de trigo"
        ///                 }
        ///             ]
        ///         }
        ///     ]
        ///     
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public IActionResult GetMyFavorites()
        {
            var userName = User?.Identity?.Name;
            var author = _context.Authors.SingleOrDefault(a => a.UserName == userName);
            if (author == null)
                return NotFound("Usuário não encontrado.");

            var favoriteRecipes = _context.FavoriteRecipes
                .Include(fr => fr.Recipe.Ingredients)
                .Include(fr => fr.Recipe.Steps)
                .Include(fr => fr.Recipe.Author)
                .Where(fr => fr.AuthorId == author.ID && !fr.Recipe.IsPrivate)
                .Select(fr => RecipeResponseDto.FromEntity(fr.Recipe))
                .ToList();

            return Ok(favoriteRecipes);
        }
    }
}
