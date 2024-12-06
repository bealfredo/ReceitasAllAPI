using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceitasAllAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ReceitasAllAPI.Controllers
{
    /// <summary>
    /// Controlador de receitas
    /// </summary>
    [Authorize]
    [Route("api/recipes")]
    [ApiController]
    public class RecipesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor do controlador de receitas
        /// </summary>
        /// <param name="context"></param>
        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todas as receitas públicas disponíveis.
        /// </summary>
        /// <returns>Uma lista de receitas públicas com seus ingredientes, etapas e autor.</returns>
        /// <response code="200">Lista de receitas públicas retornada com sucesso.</response>
        /// <response code="500">Erro ao obter as receitas (ex: erro de banco de dados ou falha interna).</response>
        /// <remarks>
        /// Este endpoint retorna todas as receitas que não são privadas, incluindo os ingredientes, as etapas e o autor de cada receita.
        /// 
        /// Este é um endpoint público, ou seja, não é necessário autenticação para acessá-lo.
        /// 
        /// Exemplo de retorno:
        /// 
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var recipesResponse = _context.Recipes
                                .Include(r => r.Ingredients)
                                .Include(r => r.Steps)
                                .Include(r => r.Author)
                                .Where(r => !r.IsPrivate)
                                .Select(r => RecipeResponseDto.FromEntity(r) )
                                .ToList();

            return Ok(recipesResponse);
        }

        /// <summary>
        /// Cria uma nova receita associada ao usuário autenticado.
        /// </summary>
        /// <param name="recipeDto">O objeto de dados da receita contendo informações como título, descrição, ingredientes e etapas.</param>
        /// <returns>Retorna a receita recém-criada com os detalhes incluídos.</returns>
        /// <response code="201">Receita criada com sucesso.</response>
        /// <response code="400">Se o modelo de dados fornecido for inválido.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        /// <response code="404">Se o autor (usuário) não for encontrado no sistema.</response>
        /// <remarks>
        /// Este endpoint cria uma nova receita, associando-a ao autor (usuário) autenticado.
        /// 
        /// A receita é salva no banco de dados e, em seguida, retorna a resposta com o novo recurso criado.
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        public IActionResult Create(RecipeDto recipeDto)
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

            var newRecipe = new Recipe
            {
                Title = recipeDto.Title,
                Description = recipeDto.Description,
                Image = recipeDto.Image,
                Difficulty = recipeDto.Difficulty,
                IsPrivate = recipeDto.IsPrivate,
                PreparationTimeInMinutes = recipeDto.PreparationTimeInMinutes,
                Rendimento = recipeDto.Rendimento,
                DateAdded = DateTime.Now,
                DateUpdated = DateTime.Now,
                AccentColor = recipeDto.AccentColor != null ? recipeDto.AccentColor : "#333",
                AuthorId = author.ID,
                Steps = new List<Step>(),
                Ingredients = new List<Ingredient>()
            };

            //_context.Recipes.Add(newRecipe);

            foreach (var ingredient in recipeDto.Ingredients)
            {
                newRecipe.Ingredients.Add(new Ingredient
                {
                    Order = ingredient.Order,
                    Value = ingredient.Value,
                    RecipeId = newRecipe.ID
                });
            }

            foreach (var step in recipeDto.Steps)
            {
                newRecipe.Steps.Add(new Step
                {
                    Order = step.Order,
                    Value = step.Value,
                    RecipeId = newRecipe.ID
                });
            }

            _context.Recipes.Add(newRecipe);
            _context.SaveChanges();

            var recipeResponse = RecipeResponseDto.FromEntity(newRecipe);

            return CreatedAtAction(nameof(GetById), new { id = newRecipe.ID }, recipeResponse);
        }

        /// <summary>
        /// Obtém os detalhes de uma receita pelo seu ID.
        /// </summary>
        /// <param name="id">ID da receita a ser recuperada.</param>
        /// <returns>Retorna os detalhes da receita, incluindo ingredientes, etapas e autor.</returns>
        /// <response code="200">Receita encontrada e retornada com sucesso.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        /// <response code="403">Se a receita for privada e o usuário não for o autor da receita.</response>
        /// <response code="404">Se a receita com o ID fornecido não for encontrada.</response>
        /// <remarks>
        /// Este endpoint retorna os detalhes de uma receita específica, incluindo os ingredientes e etapas associadas. Se a receita for privada,
        /// apenas o autor da receita pode acessá-la.
        /// 
        /// Para receitas públicas, qualquer usuário autenticado pode acessar os detalhes da receita.
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
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
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

            // check if the recipe is private and if the user is the author
            var userName = User?.Identity?.Name;

            if (recipe.IsPrivate && recipe.Author.UserName != userName)
            {
                return StatusCode(403, "Você não tem autorização para acessar essa receita privada.");
            }

            return Ok(RecipeResponseDto.FromEntity(recipe));
        }

        /// <summary>
        /// Atualiza uma receita existente com os dados fornecidos.
        /// </summary>
        /// <param name="id">ID da receita a ser atualizada.</param>
        /// <param name="recipeDto">Objeto contendo os dados atualizados da receita.</param>
        /// <returns>Retorna os detalhes da receita atualizada.</returns>
        /// <response code="200">Receita atualizada com sucesso.</response>
        /// <response code="400">Se o modelo de dados fornecido não for válido.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        /// <response code="403">Se o usuário não for o autor da receita e tentar editá-la.</response>
        /// <response code="404">Se a receita com o ID fornecido não for encontrada.</response>
        /// <remarks>
        /// Este endpoint permite que o autor de uma receita atualize os detalhes da receita, incluindo título, descrição, imagem, ingredientes, 
        /// etapas e outros dados associados.
        /// 
        /// A receita só pode ser editada pelo seu autor.
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public IActionResult Update(int id, RecipeDto recipeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipe = _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Author)
                .FirstOrDefault(r => r.ID == id);

            if (recipe == null)
            {
                return NotFound();
            }

            // check if the recipe is private and if the user is the author
            var userName = User?.Identity?.Name;

            if (recipe.Author.UserName != userName)
            {
                return StatusCode(403, "Você não tem autorização para editar uma receita que não é sua.");
            }

            recipe.Title = recipeDto.Title;
            recipe.Description = recipeDto.Description;
            recipe.Image = recipeDto.Image;
            recipe.Difficulty = recipeDto.Difficulty;
            recipe.IsPrivate = recipeDto.IsPrivate;
            recipe.PreparationTimeInMinutes = recipeDto.PreparationTimeInMinutes;
            recipe.Rendimento = recipeDto.Rendimento;
            recipe.DateUpdated = DateTime.Now;
            recipe.AccentColor = recipeDto.AccentColor;

            // update ingredients
            foreach (var ingredient in recipe.Ingredients)
            {
                _context.Ingredients.Remove(ingredient);
            }

            recipe.Ingredients.Clear();

            foreach (var ingredient in recipeDto.Ingredients)
            {
                recipe.Ingredients.Add(new Ingredient
                {
                    Order = ingredient.Order,
                    Value = ingredient.Value,
                    RecipeId = recipe.ID
                });
            }

            // update steps
            foreach (var step in recipe.Steps)
            {
                _context.Steps.Remove(step);
            }

            recipe.Steps.Clear();

            foreach (var step in recipeDto.Steps)
            {
                recipe.Steps.Add(new Step
                {
                    Order = step.Order,
                    Value = step.Value,
                    RecipeId = recipe.ID
                });
            }

            _context.SaveChanges();

            return Ok(RecipeResponseDto.FromEntity(recipe));
        }

        /// <summary>
        /// Deleta uma receita existente com base no ID fornecido,, removendo também todas as referências a essa receita em livros de receitas e favoritos.
        /// </summary>
        /// <param name="id">ID da receita a ser deletada.</param>
        /// <returns>Retorna o status da operação.</returns>
        /// <response code="204">Receita deletada com sucesso.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        /// <response code="403">Se o usuário não for o autor da receita e tentar deletá-la.</response>
        /// <response code="404">Se a receita com o ID fornecido não for encontrada.</response>
        /// <remarks>
        /// Este endpoint permite que o autor de uma receita a exclua.
        /// 
        /// A receita é removida do banco de dados e todas as referências a ela, como livros de receitas e favoritos, são removidas.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
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

            // check if the recipe is private and if the user is the author
            var userName = User?.Identity?.Name;

            if (recipe.Author.UserName != userName)
            {
                return StatusCode(403, "Você não tem autorização para deletar uma receita que não é sua.");
            }

            // remover de todos os livros de receitas
            _context.RecipeCookbooks.RemoveRange(_context.RecipeCookbooks.Where(rc => rc.RecipeId == id));

            // remover de todos os favoritos
            _context.FavoriteRecipes.RemoveRange(_context.FavoriteRecipes.Where(fr => fr.RecipeId == id));

            _context.Recipes.Remove(recipe);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Retorna todas as receitas do usuário autenticado.
        /// </summary>
        /// <returns>Uma lista das receitas do usuário autenticado.</returns>
        /// <response code="200">Lista de receitas do usuário autenticado.</response>
        /// <remarks>
        /// Este endpoint permite que um usuário autenticado obtenha todas as suas próprias receitas.
        ///
        /// As receitas retornadas incluem os ingredientes, etapas e o autor.
        /// 
        /// Exemplo de retorno:
        /// 
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
        [HttpGet("myrecipes")]
        public IActionResult GetMyRecipes()
        {
            var userName = User?.Identity?.Name;

            var recipesResponse = _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Author)
                .Where(r => r.Author.UserName == userName)
                .Select(r => RecipeResponseDto.FromEntity(r))
                .ToList();

            return Ok(recipesResponse);
        }


    }
}
