using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ReceitasAllAPI.Dtos;
using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Persistence;

namespace ReceitasAllAPI.Controllers
{
    /// <summary>
    /// Controlador de livros de receitas
    /// </summary>
    [Authorize]
    [Route("api/cookbooks")]
    [ApiController]
    public class CookbooksController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor do controlador de livros de receitas
        /// </summary>
        /// <param name="context"></param>
        public CookbooksController(ApplicationDbContext context) => _context = context;

        /// <summary>
        /// Recupera todos os livros de receitas públicos.
        /// </summary>
        /// <returns>Uma lista de livros de receitas públicos.</returns>
        /// <response code="200">A lista de livros de receitas públicos foi retornada com sucesso.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint permite recuperar todos os livros de receitas que são públicos.
        /// 
        /// Este é um endpoint público, ou seja, não requer autenticação.
        /// 
        /// Os livros de receitas são retornados com seus respectivos autores, receitas, ingredientes e etapas.
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var recipesResponse = _context.Cookbooks
                                .Include(c => c.Author)
                                .Include(c => c.RecipeCookbooks)
                                    .ThenInclude(rc => rc.Recipe)
                                        .ThenInclude(r => r.Ingredients)
                                .Include(r => r.RecipeCookbooks)
                                    .ThenInclude(rc => rc.Recipe)
                                        .ThenInclude(r => r.Steps)
                                .Where(c => !c.IsPrivate)
                                .Select(c => CookbookResponseDto.FromEntity(c))
                                .ToList();

            // remove private recipes from the response
            foreach (var cookbook in recipesResponse)
            {
                cookbook.RecipeCookbook = cookbook.RecipeCookbook.Where(rc => !rc.Recipe.IsPrivate).ToList();
            }

            return Ok(recipesResponse);
        }

        /// <summary>
        /// Cria um novo livro de receitas associado ao autor autenticado.
        /// </summary>
        /// <param name="cookbookDto">Objeto contendo as informações do novo livro de receitas.</param>
        /// <returns>Resultado da operação de criação do livro de receitas.</returns>
        /// <response code="201">Livro de receitas criado com sucesso.</response>
        /// <response code="400">A requisição é inválida. Possíveis razões incluem:
        /// Uma das receitas informadas não foi encontrada;
        /// Uma das receitas informadas não pertence ao usuário autenticado;
        /// Receitas duplicadas no livro de receitas;
        /// </response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint permite que o autor autenticado crie um novo livro de receitas, 
        /// incluindo receitas associadas ao livro. Somente o autor de uma receita pode 
        /// adicioná-la ao livro de receitas.
        /// 
        /// Apenas as receitas do autor autenticado podem ser adicionadas ao livro de receitas.
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
        /// 
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public IActionResult Create(CookbookDto cookbookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized("Usuário não autenticado.");
            }

            // author is the authenticated user
            var author = _context.Authors.SingleOrDefault(a => a.UserName == User.Identity.Name);

            if (author == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var newCookbook = new Cookbook
            {
                Title = cookbookDto.Title,
                Description = cookbookDto.Description,
                Image = cookbookDto.Image,
                DateAdded = DateTime.Now,
                DateUpdated = DateTime.Now,
                AccentColor = cookbookDto.AccentColor != null ? cookbookDto.AccentColor : "#333",
                AuthorId = author.ID,
                RecipeCookbooks = new List<RecipeCookbook>()
            };

            //_context.Recipes.Add(newRecipe);

            foreach (var recipeCookbookDto in cookbookDto.Recipes)
            {
                var recipe = _context.Recipes
                    .Include(r => r.Author)
                    .Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .SingleOrDefault(r => r.ID == recipeCookbookDto.RecipeId);
                if (recipe == null)
                    return BadRequest("Uma das receitas informadas não foi encontrada. RecipeId: " + recipeCookbookDto.RecipeId);
                else if (recipe.AuthorId != author.ID)
                    return BadRequest("Uma das receitas informadas não pertence ao usuário autenticado. RecipeId: " + recipeCookbookDto.RecipeId);
                else if (newCookbook.RecipeCookbooks.Any(rc => rc.RecipeId == recipeCookbookDto.RecipeId))
                    return BadRequest("Não é possível ter receitas duplicadas no livro de receitas. RecipeId: " + recipeCookbookDto.RecipeId);

                newCookbook.RecipeCookbooks.Add(new RecipeCookbook
                {
                    RecipeId = recipeCookbookDto.RecipeId,
                    CookbookId = newCookbook.ID,
                    Order = recipeCookbookDto.Order,
                    DateAdded = DateTime.Now
                });
            }

            _context.Cookbooks.Add(newCookbook);
            _context.SaveChanges();

            var cookbookResponse = CookbookResponseDto.FromEntity(newCookbook);

            return CreatedAtAction(nameof(GetById), new { id = newCookbook.ID }, cookbookResponse);
        }

        /// <summary>
        /// Obtém um livro de receitas pelo ID.
        /// </summary>
        /// <param name="id">ID do livro de receitas a ser recuperado.</param>
        /// <returns>Detalhes do livro de receitas.</returns>
        /// <response code="200">Retorna o livro de receitas com sucesso.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="404">Livro de receitas não encontrado.</response>
        /// <response code="403">Acesso não autorizado ao livro de receitas privado.</response>
        /// <remarks>
        /// Este endpoint permite que um usuário recupere detalhes de um livro de receitas 
        /// pelo seu ID. Se o livro for privado, o usuário precisa ser o autor do livro 
        /// para visualizá-lo.
        /// 
        /// Apenas receitas públicas são retornadas no livro de receitas se o usuário não for o autor.
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
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

            // check if the recipe is private and if the user is the author
            var userName = User?.Identity?.Name;
            if (cookbook.IsPrivate && cookbook.Author.UserName != userName)
            {
                return StatusCode(403, "Você não tem autorização para acessar esse livro de receitas privado.");
            }

            var cookbookResponse = CookbookResponseDto.FromEntity(cookbook);

            // remove private recipes from the response if the user is not the author
            if (cookbook.Author.UserName != userName)
            {
                cookbookResponse.RecipeCookbook = cookbookResponse.RecipeCookbook.Where(rc => !rc.Recipe.IsPrivate).ToList();
            }
            
            return Ok(cookbookResponse);
        }

        /// <summary>
        /// Atualiza os detalhes de um livro de receitas.
        /// </summary>
        /// <param name="id">ID do livro de receitas a ser atualizado.</param>
        /// <param name="cookbookDto">Objeto contendo os novos dados para o livro de receitas.</param>
        /// <returns>Resposta indicando o sucesso ou falha da operação de atualização.</returns>
        /// <response code="204">Livro de receitas atualizado com sucesso.</response>
        /// <response code="400">Erro ao tentar atualizar o livro de receitas (ex: dados inválidos ou erro de transação).</response>
        /// <response code="403">Usuário não tem permissão para editar este livro de receitas.</response>
        /// <response code="404">Livro de receitas não encontrado.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <remarks>
        /// Este endpoint permite que o autor de um livro de receitas atualize os detalhes do livro, incluindo o título, 
        /// descrição, imagem, cor de destaque e receitas associadas. Além disso, é realizada uma validação para garantir que 
        /// o usuário autenticado seja o autor do livro e que não haja receitas duplicadas na atualização.
        /// 
        /// Apenas receitas do autor autenticado podem ser adicionadas ao livro de receitas.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{id}")]
        public IActionResult Update(int id, CookbookDto cookbookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

                if (cookbook.Author.UserName != userName)
                {
                    return StatusCode(403, "Você não tem autorização para editar esse livro de receitas.");
                }

                cookbook.Title = cookbookDto.Title;
                cookbook.Description = cookbookDto.Description;
                cookbook.Image = cookbookDto.Image;
                cookbook.DateUpdated = DateTime.Now;
                cookbook.AccentColor = cookbookDto.AccentColor != null ? cookbookDto.AccentColor : "#333";

                // remove all recipes from the cookbook that are not in the new list
                _context.RecipeCookbooks.RemoveRange(cookbook.RecipeCookbooks.Where(rc => !cookbookDto.Recipes.Any(r => r.RecipeId == rc.RecipeId)));

                var newRecipeCookbooks = cookbookDto.Recipes.Where(r => !cookbook.RecipeCookbooks.Any(rc => rc.RecipeId == r.RecipeId)).ToList();

                // check if theers no duplicated recipes in dto
                foreach (var recipeCookbookDto in cookbookDto.Recipes)
                {
                    if (cookbookDto.Recipes.Count(rc => rc.RecipeId == recipeCookbookDto.RecipeId) > 1)
                        return BadRequest("Não é possível ter receitas duplicadas no livro de receitas. RecipeId: " + recipeCookbookDto.RecipeId);
                }


                foreach (var recipeCookbookDto in newRecipeCookbooks)
                {
                    var recipe = _context.Recipes
                        .Include(r => r.Author)
                        .Include(r => r.Ingredients)
                        .Include(r => r.Steps)
                        .SingleOrDefault(r => r.ID == recipeCookbookDto.RecipeId);
                    if (recipe == null)
                        return BadRequest("Uma das receitas informadas não foi encontrada. RecipeId: " + recipeCookbookDto.RecipeId);
                    else if (recipe.AuthorId != cookbook.AuthorId)
                        return BadRequest("Uma das receitas informadas não pertence ao usuário autenticado. RecipeId: " + recipeCookbookDto.RecipeId);
                    else if (cookbook.RecipeCookbooks.Any(rc => rc.RecipeId == recipeCookbookDto.RecipeId))
                        return BadRequest("Não é possível ter receitas duplicadas no livro de receitas. RecipeId: " + recipeCookbookDto.RecipeId);

                    cookbook.RecipeCookbooks.Add(new RecipeCookbook
                    {
                        RecipeId = recipeCookbookDto.RecipeId,
                        CookbookId = cookbook.ID,
                        Order = recipeCookbookDto.Order,
                        DateAdded = DateTime.Now
                    });
                }

                _context.Cookbooks.Update(cookbook);
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex) {
                transaction.Rollback();
                return StatusCode(400, "Erro ao atualizar o livro de receitas.");
            }

        }

        /// <summary>
        /// Remove um livro de receitas pelo seu ID.
        /// </summary>
        /// <param name="id">ID do livro de receitas a ser removido.</param>
        /// <returns>Resposta indicando o sucesso ou falha da operação de remoção.</returns>
        /// <response code="204">Livro de receitas removido com sucesso.</response>
        /// <response code="400">Erro ao tentar remover o livro de receitas (ex: erro de transação).</response>
        /// <response code="403">Usuário não tem permissão para remover este livro de receitas.</response>
        /// <response code="404">Livro de receitas não encontrado.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <remarks>
        /// Este endpoint permite que o autor de um livro de receitas remova o livro de receitas.
        /// 
        /// As receitas associadas ao livro de receitas não são removidas do sistema.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
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

                if (cookbook.Author.UserName != userName)
                {
                    return StatusCode(403, "Você não tem autorização para remover esse livro de receitas.");
                }

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
        /// Retorna todos os livros de receitas do usuário autenticado.
        /// </summary>
        /// <returns>Uma lista dos livros de receitas do usuário autenticado.</returns>
        /// <response code="200">Lista de livros de receitas do usuário autenticado.</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <remarks>
        /// Este endpoint permite que um usuário autenticado obtenha todos os seus próprios livros de receitas.
        ///
        /// Cada livro de receitas retornado inclui as receitas associadas, com os ingredientes e etapas de cada receita.
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("mycookbooks")]
        public IActionResult GetMyCookbooks()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized("Usuário não autenticado.");
            }

            var userName = User.Identity.Name;

            var cookbooks = _context.Cookbooks
                .Include(c => c.Author)
                .Include(c => c.RecipeCookbooks)
                    .ThenInclude(rc => rc.Recipe)
                        .ThenInclude(r => r.Ingredients)
                .Include(r => r.RecipeCookbooks)
                    .ThenInclude(rc => rc.Recipe)
                        .ThenInclude(r => r.Steps)
                .Where(c => c.Author.UserName == userName)
                .Select(c => CookbookResponseDto.FromEntity(c))
                .ToList();

            return Ok(cookbooks);
        }

        /// <summary>
        /// Adiciona uma receita a um livro de receitas, especificando a ordem de exibição.
        /// </summary>
        /// <param name="id">ID do livro de receitas ao qual a receita será adicionada.</param>
        /// <param name="recipeId">ID da receita a ser adicionada ao livro de receitas.</param>
        /// <param name="order">A ordem de exibição da receita dentro do livro de receitas.</param>
        /// <returns>Resposta indicando o sucesso ou falha da operação de adição.</returns>
        /// <response code="204">Receita adicionada com sucesso ao livro de receitas.</response>
        /// <response code="400">Erro ao adicionar a receita ao livro de receitas (ex: erro de transação ou receita não pertence ao usuário).</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="403">Usuário não tem permissão para adicionar receitas a este livro de receitas.</response>
        /// <response code="404">Livro de receitas ou receita não encontrado.</response>
        /// <response code="409">A receita já está presente no livro de receitas.</response>
        /// <remarks>
        /// Este endpoint permite que o autor de um livro de receitas adicione uma receita existente 
        /// ao livro, especificando a ordem em que a receita será exibida.
        /// 
        /// Apenas receitas do autor autenticado podem ser adicionadas ao livro de receitas.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost("{id}/recipe/{recipeId}/add-with-order/{order}")]
        public IActionResult AddRecipe(int id, int recipeId, int order)
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

                if (cookbook.Author.UserName != userName)
                {
                    return StatusCode(403, "Você não tem autorização para adicionar receitas a esse livro de receitas.");
                }

                var recipe = _context.Recipes
                    .Include(r => r.Author)
                    .Include(r => r.Ingredients)
                    .Include(r => r.Steps)
                    .SingleOrDefault(r => r.ID == recipeId);

                if (recipe == null)
                {
                    return NotFound("Receita não encontrada.");
                }

                if (recipe.AuthorId != cookbook.AuthorId)
                {
                    return BadRequest("A receita informada não pertence ao usuário autenticado.");
                }

                if (cookbook.RecipeCookbooks.Any(rc => rc.RecipeId == recipeId))
                {
                    return Conflict("A receita já está no livro de receitas.");
                }

                cookbook.RecipeCookbooks.Add(new RecipeCookbook
                {
                    RecipeId = recipeId,
                    CookbookId = cookbook.ID,
                    Order = order,
                    DateAdded = DateTime.Now
                });

                _context.Cookbooks.Update(cookbook);
                _context.SaveChanges();
                transaction.Commit();

                return NoContent();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(400, "Erro ao adicionar a receita ao livro de receitas.");
            }            
        }

        /// <summary>
        /// Remove uma receita de um livro de receitas.
        /// </summary>
        /// <param name="id">ID do livro de receitas de onde a receita será removida.</param>
        /// <param name="recipeId">ID da receita que será removida do livro de receitas.</param>
        /// <returns>Resposta indicando o sucesso ou falha da operação de remoção.</returns>
        /// <response code="204">Receita removida com sucesso do livro de receitas.</response>
        /// <response code="400">Erro ao remover a receita do livro de receitas (ex: erro de transação ou falha ao encontrar a receita).</response>
        /// <response code="401">Usuário não autenticado.</response>
        /// <response code="403">Usuário não tem permissão para remover receitas deste livro de receitas.</response>
        /// <response code="404">Livro de receitas ou receita não encontrada.</response>
        /// <remarks>
        /// Este endpoint permite que o autor de um livro de receitas remova uma receita do livro.
        /// 
        /// A receita não é removida do sistema, apenas do livro de receitas.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}/recipe/{recipeId}")]
        public IActionResult RemoveRecipe(int id, int recipeId)
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

                if (cookbook.Author.UserName != userName)
                {
                    return StatusCode(403, "Você não tem autorização para remover receitas desse livro de receitas.");
                }

                var recipeCookbook = cookbook.RecipeCookbooks.SingleOrDefault(rc => rc.RecipeId == recipeId);

                if (recipeCookbook == null)
                {
                    return NotFound("Receita não encontrada no livro de receitas.");
                }

                _context.RecipeCookbooks.Remove(recipeCookbook);
                _context.SaveChanges();

                transaction.Commit();

                return NoContent();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(400, "Erro ao remover a receita do livro de receitas.");
            }
        }


    }
}
