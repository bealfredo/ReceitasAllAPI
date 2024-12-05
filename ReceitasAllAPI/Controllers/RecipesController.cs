using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceitasAllAPI.Persistence;
using Microsoft.AspNetCore.Authorization;
using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ReceitasAllAPI.Controllers
{
    [Authorize]
    [Route("api/recipes")]
    [ApiController]
    public class RecipesController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

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

        [AllowAnonymous]
        [HttpGet("admin")]
        public IActionResult GetAllAdmin()
        {
            var recipesResponse = _context.Recipes
                                .Include(r => r.Ingredients)
                                .Include(r => r.Steps)
                                .Include(r => r.Author)
                                .Select(r => RecipeResponseDto.FromEntity(r))
                                .ToList();

            return Ok(recipesResponse);
        }


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
                DateAdded = recipeDto.DateAdded,
                DateUpdated = recipeDto.DateUpdated,
                AccentColor = recipeDto.AccentColor,
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

        [HttpPut]
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
            recipe.DateUpdated = recipeDto.DateUpdated;
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

        [HttpDelete]
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

            _context.Recipes.Remove(recipe);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("admin")]
        public IActionResult DeleteAdmin(int id)
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

            _context.Recipes.Remove(recipe);
            _context.SaveChanges();

            return NoContent();
        }

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
