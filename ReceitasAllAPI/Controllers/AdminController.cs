using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceitasAllAPI.Dtos;
using ReceitasAllAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ReceitasAllAPI.Controllers
{
    /// <summary>
    /// Controlador usado pelo administrador para moderação
    /// </summary>
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor do controlador de autores
        /// </summary>
        /// <param name="context"></param>
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Recupera todos os autores cadastrados no sistema.
        /// </summary>
        /// <returns>Uma lista de autores.</returns>
        /// <response code="200">A lista de autores foi retornada com sucesso.</response>
        /// <response code="401">Não autorizado. O token de acesso não é válido ou não foi fornecido.</response>
        /// <response code="403">Proibido. O usuário não tem permissão para acessar este recurso.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint retorna uma lista contendo informações de todos os autores registrados no sistema.
        /// 
        /// Exemplo de retorno:
        /// 
        /// <code>
        /// [
        ///   {
        ///     "id": 1,
        ///     "userName": "admin",
        ///     "admin": true,
        ///     "firstName": "Admin",
        ///     "lastName": "Administrador",
        ///     "nacionality": "Brasileiro",
        ///     "image": "",
        ///     "bibliography": "Administrador do sistema",
        ///     "pseudonym": "admin",
        ///     "emailContact": "admin@gmail.com"
        ///   }
        /// ]
        /// </code>
        /// 
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("authors")]
        public IActionResult GetAllAuthors()
        {
            var authorsResponse = new List<AuthorResponseDto>();

            foreach (var author in _context.Authors)
            {
                authorsResponse.Add(AuthorResponseDto.FromEntity(author));
            }

            return Ok(authorsResponse);
        }
        /// <summary>
        /// Recupera os detalhes de um autor específico pelo ID.
        /// </summary>
        /// <param name="id">ID do autor a ser recuperado.</param>
        /// <returns>Os detalhes do autor solicitado.</returns>
        /// <response code="200">O autor foi encontrado e retornado com sucesso.</response>
        /// <response code="403">Proibido. O usuário não tem permissão para acessar este recurso.</response>
        /// <response code="404">O autor com o ID especificado não foi encontrado.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint permite obter as informações detalhadas de um autor específico com base no seu ID.
        /// 
        /// Exemplo de retorno:
        /// 
        /// <code>
        /// {
        ///   "id": 1,
        ///   "userName": "admin",
        ///   "admin": true,
        ///   "firstName": "Admin",
        ///   "lastName": "Administrador",
        ///   "nacionality": "Brasileiro",
        ///   "image": "",
        ///   "bibliography": "Administrador do sistema",
        ///   "pseudonym": "admin",
        ///   "emailContact": "admin@gmail.com"
        /// }
        /// </code>
        /// 
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("authors/{id}")]
        public IActionResult GetAuthorById(int id)
        {
            var author = _context.Authors.SingleOrDefault(a => a.ID == id);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(AuthorResponseDto.FromEntity(author));
        }

        /// <summary>
        /// Obtém todos os livros de receitas, públicos e privados.
        /// </summary>
        /// <returns>Retorna uma lista de livros de receitas, incluindo seus autores, receitas associadas, ingredientes e etapas.</returns>
        /// <response code="200">Lista de livros de receitas obtida com sucesso.</response>
        /// <response code="403">Proibido. O usuário não tem permissão para acessar este recurso.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        /// <remarks>
        /// Este endpoint retorna todos os livros de receitas disponíveis no sistema, incluindo os detalhes das receitas associadas, como ingredientes e etapas.
        /// 
        /// Exemplo de retorno:
        /// 
        /// <code>
        /// [
        ///   {
        ///     "id": 106,
        ///     "title": "Bolos Deliciosos: Receitas Simples",
        ///     "description": "Bolos Deliciosos é um guia prático que traz uma seleção de receitas irresistíveis para quem ama a confeitaria. Este livro apresenta bolos fofinhos e saborosos, com um passo a passo claro e fácil de seguir, tornando a preparação acessível a todos. ",
        ///     "image": "",
        ///     "isPrivate": false,
        ///     "dateAdded": "2024-11-04T10:00:00",
        ///     "dateUpdated": "2024-11-04T10:15:00",
        ///     "accentColor": "#503422",
        ///     "author": {
        ///       "id": 2,
        ///       "userName": "mary",
        ///       "admin": false,
        ///       "firstName": "Maria",
        ///       "lastName": "da Silva",
        ///       "nacionality": "Brasileira",
        ///       "image": "",
        ///       "bibliography": "Uma mulher que adora cozinhar e compartilhar suas receitas",
        ///       "pseudonym": "mary",
        ///       "emailContact": "maria@gmail.com"
        ///     },
        ///     "recipeCookbook": [
        ///       {
        ///         "id": 1,
        ///         "recipe": {
        ///           "id": 101,
        ///           "title": "Bolo de Chocolate",
        ///           "description": "Como não pensar em um bolo de chocolate com morango e não salivar, certo? O bolo sensação é o doce que conquista o paladar de muita gente, já que o morango quebra o gosto adocicado do chocolate.",
        ///           "image": "",
        ///           "difficulty": 1,
        ///           "isPrivate": false,
        ///           "preparationTimeInMinutes": 45,
        ///           "rendimento": "8 porções",
        ///           "dateAdded": "2024-10-03T08:15:33",
        ///           "dateUpdated": "2024-10-04T09:12:33",
        ///           "accentColor": "#8B4513",
        ///           "author": {
        ///             "id": 2,
        ///             "userName": "mary",
        ///             "admin": false,
        ///             "firstName": "Maria",
        ///             "lastName": "da Silva",
        ///             "nacionality": "Brasileira",
        ///             "image": "",
        ///             "bibliography": "Uma mulher que adora cozinhar e compartilhar suas receitas",
        ///             "pseudonym": "mary",
        ///             "emailContact": "maria@gmail.com"
        ///           },
        ///           "steps": [
        ///             {
        ///               "id": 1,
        ///               "order": 1,
        ///               "value": "Em uma batedeira, bata as claras em neve."
        ///             }
        ///           ],
        ///           "ingredients": [
        ///             {
        ///               "id": 1,
        ///               "order": 1,
        ///               "value": "2 xícaras de farinha de trigo"
        ///             }
        ///           ]
        ///         },
        ///         "order": 1,
        ///         "dateAdded": "2024-11-04T10:10:00"
        ///       }
        ///     ]
        ///   }
        /// ]
        /// </code>
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("cookbooks")]
        public IActionResult GetAllCookbooks()
        {
            var recipesResponse = _context.Cookbooks
                                .Include(c => c.Author)
                                .Include(c => c.RecipeCookbooks)
                                    .ThenInclude(rc => rc.Recipe)
                                        .ThenInclude(r => r.Ingredients)
                                .Include(r => r.RecipeCookbooks)
                                    .ThenInclude(rc => rc.Recipe)
                                        .ThenInclude(r => r.Steps)
                                .Select(c => CookbookResponseDto.FromEntity(c))
                                .ToList();

            return Ok(recipesResponse);
        }

        /// <summary>
        /// Obtém um livro de receitas pelo ID.
        /// </summary>
        /// <param name="id">ID do livro de receitas a ser recuperado.</param>
        /// <returns>Detalhes do livro de receitas.</returns>
        /// <response code="200">Retorna o livro de receitas com sucesso.</response>
        /// <response code="403">Proibido. O usuário não tem permissão para acessar este recurso.</response>
        /// <response code="404">Livro de receitas não encontrado.</response>
        /// <remarks>
        /// Este endpoint permite que um administrado recupere detalhes de um livro de receitas 
        /// pelo seu ID. Caso o livro não seja encontrado, retorna um código de erro 404.
        /// 
        /// O livro pode ser público ou privado e de qualquer autor.
        /// 
        /// Exemplo de retorno:
        /// 
        /// <code>
        ///   {
        ///     "id": 106,
        ///     "title": "Bolos Deliciosos: Receitas Simples",
        ///     "description": "Bolos Deliciosos é um guia prático que traz uma seleção de receitas irresistíveis para quem ama a confeitaria. Este livro apresenta bolos fofinhos e saborosos, com um passo a passo claro e fácil de seguir, tornando a preparação acessível a todos. ",
        ///     "image": "",
        ///     "isPrivate": false,
        ///     "dateAdded": "2024-11-04T10:00:00",
        ///     "dateUpdated": "2024-11-04T10:15:00",
        ///     "accentColor": "#503422",
        ///     "author": {
        ///       "id": 2,
        ///       "userName": "mary",
        ///       "admin": false,
        ///       "firstName": "Maria",
        ///       "lastName": "da Silva",
        ///       "nacionality": "Brasileira",
        ///       "image": "",
        ///       "bibliography": "Uma mulher que adora cozinhar e compartilhar suas receitas",
        ///       "pseudonym": "mary",
        ///       "emailContact": "maria@gmail.com"
        ///     },
        ///     "recipeCookbook": [
        ///       {
        ///         "id": 1,
        ///         "recipe": {
        ///           "id": 101,
        ///           "title": "Bolo de Chocolate",
        ///           "description": "Como não pensar em um bolo de chocolate com morango e não salivar, certo? O bolo sensação é o doce que conquista o paladar de muita gente, já que o morango quebra o gosto adocicado do chocolate.",
        ///           "image": "",
        ///           "difficulty": 1,
        ///           "isPrivate": false,
        ///           "preparationTimeInMinutes": 45,
        ///           "rendimento": "8 porções",
        ///           "dateAdded": "2024-10-03T08:15:33",
        ///           "dateUpdated": "2024-10-04T09:12:33",
        ///           "accentColor": "#8B4513",
        ///           "author": {
        ///             "id": 2,
        ///             "userName": "mary",
        ///             "admin": false,
        ///             "firstName": "Maria",
        ///             "lastName": "da Silva",
        ///             "nacionality": "Brasileira",
        ///             "image": "",
        ///             "bibliography": "Uma mulher que adora cozinhar e compartilhar suas receitas",
        ///             "pseudonym": "mary",
        ///             "emailContact": "maria@gmail.com"
        ///           },
        ///           "steps": [
        ///             {
        ///               "id": 1,
        ///               "order": 1,
        ///               "value": "Em uma batedeira, bata as claras em neve."
        ///             }
        ///           ],
        ///           "ingredients": [
        ///             {
        ///               "id": 1,
        ///               "order": 1,
        ///               "value": "2 xícaras de farinha de trigo"
        ///             }
        ///           ]
        ///         },
        ///         "order": 1,
        ///         "dateAdded": "2024-11-04T10:10:00"
        ///       }
        ///     ]
        ///   }
        /// </code>
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("cookbook/{id}")]
        public IActionResult GetCookbookById(int id)
        {
            var cookbook = _context.Cookbooks
                .Include(c => c.Author)
                .Include(c => c.RecipeCookbooks)
                    .ThenInclude(rc => rc.Recipe)
                        .ThenInclude(r => r.Ingredients)
                .Include(r => r.RecipeCookbooks)
                    .ThenInclude(rc => rc.Recipe)
                        .ThenInclude(r => r.Steps)
                .SingleOrDefault(c => c.ID == id);


            if (cookbook == null)
            {
                return NotFound("Livro de receitas não encontrado.");
            }

            return Ok(CookbookResponseDto.FromEntity(cookbook));
        }

        /// <summary>
        /// Remove um livro de receitas pelo seu ID.
        /// </summary>
        /// <param name="id">ID do livro de receitas a ser removido.</param>
        /// <returns>Resposta indicando o sucesso ou falha da operação de remoção.</returns>
        /// <response code="204">Livro de receitas removido com sucesso.</response>
        /// <response code="400">Erro ao tentar remover o livro de receitas (ex: erro de transação).</response>
        /// <response code="403">Proibido. O usuário não tem permissão para acessar este recurso.</response>
        /// <response code="404">Livro de receitas não encontrado.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <remarks>
        /// Este endpoint permite que um administrador remova um livro de receitas do sistema.
        /// 
        /// As receitas do livro de receitas não são removidas do sistema.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("cookbook/{id}")]
        public IActionResult DeleteCookbook(int id)
        {
            // use transaction to rollback if something goes wrong
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var cookbook = _context.Cookbooks
                .Include(c => c.Author)
                .Include(c => c.RecipeCookbooks)
                    .ThenInclude(rc => rc.Recipe)
                        .ThenInclude(r => r.Ingredients)
                .Include(r => r.RecipeCookbooks)
                    .ThenInclude(rc => rc.Recipe)
                        .ThenInclude(r => r.Steps)
                .SingleOrDefault(c => c.ID == id);

                if (cookbook == null)
                {
                    return NotFound("Livro de receitas não encontrado.");
                }

                if (User?.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized("Usuário não autenticado.");
                }

                var userName = User.Identity.Name;

                // remover recipeCookbooks
                _context.RecipeCookbooks.RemoveRange(_context.RecipeCookbooks.Where(rc => rc.CookbookId == cookbook.ID));

                _context.Cookbooks.Remove(cookbook);
                _context.SaveChanges();

                transaction.Commit();

                return NoContent();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(400, "Erro ao remover o livro de receitas.");
            }
        }

        /// <summary>
        /// Obtém todas as receitas, incluindo privadas e públicas..
        /// </summary>
        /// <returns>Uma lista de todas as receitas com seus ingredientes, etapas e autor.</returns>
        /// <response code="200">Lista de todas as receitas retornada com sucesso.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="403">Proibido. O usuário não tem permissão para acessar este recurso.</response>
        /// <response code="500">Erro ao obter as receitas (ex: erro de banco de dados ou falha interna).</response>
        /// <remarks>
        /// Este endpoint retorna todas as receitas, sem restrição de privacidade, públicas ou privadas.
        /// 
        /// A resposta inclui os ingredientes, as etapas e o autor de cada receita.
        /// 
        /// Exemplo de retorno:
        /// <code>
        /// [
        ///     {
        ///         "id": 101,
        ///         "title": "Bolo de Chocolate",
        ///         "description": "Como não pensar em um bolo de chocolate com morango e não salivar, certo? O bolo sensação é o doce que conquista o paladar de muita gente, já que o morango quebra o gosto adocicado do chocolate.",
        ///         "image": "",
        ///         "difficulty": 1,
        ///         "isPrivate": false,
        ///         "preparationTimeInMinutes": 45,
        ///         "rendimento": "8 porções",
        ///         "dateAdded": "2024-10-03T08:15:33",
        ///         "dateUpdated": "2024-10-04T09:12:33",
        ///         "accentColor": "#8B4513",
        ///         "author": {
        ///             "id": 2,
        ///             "userName": "mary",
        ///             "admin": false,
        ///             "firstName": "Maria",
        ///             "lastName": "da Silva",
        ///             "nacionality": "Brasileira",
        ///             "image": "",
        ///             "bibliography": "Uma mulher que adora cozinhar e compartilhar suas receitas",
        ///             "pseudonym": "mary",
        ///             "emailContact": "maria@gmail.com"
        ///         },
        ///         "steps": [
        ///             {
        ///                 "id": 1,
        ///                 "order": 1,
        ///                 "value": "Em uma batedeira, bata as claras em neve."
        ///             }
        ///         ],
        ///         "ingredients": [
        ///             {
        ///                 "id": 1,
        ///                 "order": 1,
        ///                 "value": "2 xícaras de farinha de trigo"
        ///             }
        ///         ]
        ///     }
        /// ]
        /// </code>
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("recipes")]
        public IActionResult GetAllRecipes()
        {
            var recipesResponse = _context.Recipes
                                .Include(r => r.Ingredients)
                                .Include(r => r.Steps)
                                .Include(r => r.Author)
                                .Select(r => RecipeResponseDto.FromEntity(r))
                                .ToList();

            return Ok(recipesResponse);
        }

        /// <summary>
        /// Retorna uma receita pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da receita a ser retornada.</param>
        /// <returns>Detalhes da receita solicitada.</returns>
        /// <response code="200">Detalhes da receita com o ID especificado.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="403">Proibido. O usuário não tem permissão para acessar este recurso.</response>
        /// <response code="404">Receita não encontrada.</response>
        /// <remarks>
        /// Este endpoint permite que um administrador consulte uma receita específica pelo seu ID, pública ou privada.
        /// 
        /// A resposta inclui detalhes da receita, como título, descrição, ingredientes e etapas.
        /// 
        /// Exemplo de retorno:
        /// 
        /// <code>
        /// {
        ///     "id": 101,
        ///     "title": "Bolo de Chocolate",
        ///     "description": "Como não pensar em um bolo de chocolate com morango e não salivar, certo? O bolo sensação é o doce que conquista o paladar de muita gente, já que o morango quebra o gosto adocicado do chocolate.",
        ///     "image": "",
        ///     "difficulty": 1,
        ///     "isPrivate": false,
        ///     "preparationTimeInMinutes": 45,
        ///     "rendimento": "8 porções",
        ///     "dateAdded": "2024-10-03T08:15:33",
        ///     "dateUpdated": "2024-10-04T09:12:33",
        ///     "accentColor": "#8B4513",
        ///     "author": {
        ///         "id": 2,
        ///         "userName": "mary",
        ///         "admin": false,
        ///         "firstName": "Maria",
        ///         "lastName": "da Silva",
        ///         "nacionality": "Brasileira",
        ///         "image": "",
        ///         "bibliography": "Uma mulher que adora cozinhar e compartilhar suas receitas",
        ///         "pseudonym": "mary",
        ///         "emailContact": "maria@gmail.com"
        ///     },
        ///     "steps": [
        ///         {
        ///             "id": 1,
        ///             "order": 1,
        ///             "value": "Em uma batedeira, bata as claras em neve."
        ///         }
        ///     ],
        ///     "ingredients": [
        ///         {
        ///             "id": 1,
        ///             "order": 1,
        ///             "value": "2 xícaras de farinha de trigo"
        ///         }
        ///     ]
        /// }
        /// </code>
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("recipes/{id}")]
        public IActionResult GetRecipeById(int id)
        {

            var recipe = _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Author)
                .FirstOrDefault(r => r.ID == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(RecipeResponseDto.FromEntity(recipe));
        }

        /// <summary>
        /// Deleta uma receita existente com base no ID fornecido, removendo também todas as referências a essa receita em livros de receitas e favoritos.
        /// </summary>
        /// <param name="id">ID da receita a ser deletada.</param>
        /// <returns>Retorna um código 204 (No Content) se a receita for deletada com sucesso.</returns>
        /// <response code="204">Receita deletada com sucesso.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="403">Proibido. O usuário não tem permissão para acessar este recurso.</response>
        /// <response code="404">Se a receita com o ID fornecido não for encontrada.</response>
        /// <remarks>
        /// Este endpoint permite que um administrador exclua uma receita, removendo todas as referências dessa receita em livros de receitas e favoritos.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("recipe/{id}")]
        public IActionResult Delete(int id)
        {

            var recipe = _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Author)
                .FirstOrDefault(r => r.ID == id);

            if (recipe == null)
            {
                return NotFound();
            }

            // remover de todos os livros de receitas
            _context.RecipeCookbooks.RemoveRange(_context.RecipeCookbooks.Where(rc => rc.RecipeId == id));

            // remover de todos os favoritos
            _context.FavoriteRecipes.RemoveRange(_context.FavoriteRecipes.Where(fr => fr.RecipeId == id));

            _context.Recipes.Remove(recipe);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
