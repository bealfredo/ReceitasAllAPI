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
    [Authorize]
    [Route("api/cookbooks")]
    [ApiController]
    public class CookbooksController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public CookbooksController(ApplicationDbContext context) => _context = context;

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

            return Ok(recipesResponse);
        }

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

            return Ok(CookbookResponseDto.FromEntity(cookbook));
        }

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
