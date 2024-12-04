using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceitasAllAPI.Dtos;
using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Persistence;

namespace ReceitasAllAPI.Controllers
{
    [Authorize]
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var authorsResponse = new List<AuthorResponseDto>();

            foreach (var author in _context.Authors)
            {
                authorsResponse.Add(AuthorResponseDto.FromEntity(author));
            }

            return Ok(authorsResponse);
        }


        /// <summary>
        /// Cria conta de autor (usuário da API)
        /// </summary>
        /// <param name="authorDto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create(AuthorCreateDto authorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // verificar se o usuário já existe
            var authorExists = _context.Authors.SingleOrDefault(a => a.UserName == authorDto.UserName);
            if (authorExists != null)
            {
                return Conflict("Já existe um usuário com o UserName informado");
            }

            var authorEntity = new Author
            {
                UserName = authorDto.UserName,
                PasswordHash = authorDto.Password,
                FirstName = authorDto.FirstName,
                LastName = authorDto.LastName,
                Nacionality = authorDto.Nacionality,
                Image = authorDto.Image,
                Bibliography = authorDto.Bibliography,
                Pseudonym = authorDto.Pseudonym,
                EmailContact = authorDto.EmailContact,
            };

            authorEntity.Admin = false;

            _context.Authors.Add(authorEntity);
            _context.SaveChanges();

            var authorResponse = AuthorResponseDto.FromEntity(authorEntity);

            return CreatedAtAction(nameof(GetById), new { id = authorEntity.ID }, authorResponse);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var author = _context.Authors.SingleOrDefault(a => a.ID == id);

            if (author == null)
            {
                return NotFound(new { message = "Usu" });
            }

            return Ok(AuthorResponseDto.FromEntity(author));
        }

        [HttpGet("myprofile")]
        public IActionResult MeuPerfil()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized("Usuário não autenticado.");
            }

            var userName = User.Identity.Name;

            var author = _context.Authors.SingleOrDefault(a => a.UserName == userName);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(AuthorResponseDto.FromEntity(author));
        }

        [HttpPut("myprofile/edit")]
        public IActionResult Update(AuthorUpdateDto authorDto)
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized("Usuário não autenticado.");
            }

            var userName = User.Identity.Name;

            var authorEntity = _context.Authors.SingleOrDefault(a => a.UserName == userName);

            if (authorEntity == null)
            {
                return NotFound();
            }

            authorEntity.FirstName = authorDto.FirstName;
            authorEntity.LastName = authorDto.LastName;
            authorEntity.Nacionality = authorDto.Nacionality;
            authorEntity.Image = authorDto.Image;
            authorEntity.Bibliography = authorDto.Bibliography;
            authorEntity.Pseudonym = authorDto.Pseudonym;
            authorEntity.EmailContact = authorDto.EmailContact;

            _context.Authors.Update(authorEntity);
            _context.SaveChanges();

            return NoContent();
        }

        // por enquanto não porque o usuario pode ser apago e o token ainda funcionar
        //[HttpDelete("{id}")]
        ////[Authorize(Roles = "Admin")]
        //public IActionResult Delete(int id) {
        //    var author = _context.Authors.SingleOrDefault(a => a.ID == id);

        //    if (author == null)
        //    {
        //        return NotFound();
        //    }

        //    DeleteAuthor(author);

        //    return NoContent();
        //}

        //[HttpDelete("myprofile")]
        //public IActionResult DeleteMe()
        //{
        //    if (User?.Identity?.IsAuthenticated != true)
        //    {
        //        return Unauthorized("Usuário não autenticado.");
        //    }

        //    var userName = User.Identity.Name;

        //    var author = _context.Authors.SingleOrDefault(a => a.UserName == userName);

        //    if (author == null)
        //    {
        //        return NotFound();
        //    }

        //    this.DeleteAuthor(author);

        //    return NoContent();
        //}

        //// metodo usado por Delete e DeleteMe para apagar um autor
        //private void DeleteAuthor(Entities.Author author)
        //{



        //    //// apagar receitas
        //    //var recipes = _context.Recipes.Where(r => r.AuthorId == author.ID);
        //    //_context.Recipes.RemoveRange(recipes);

        //    //// apagar favoritos
        //    //var favoriteRecipes = _context.FavoriteRecipes.Where(f => f.AuthorId == author.ID);
        //    //_context.FavoriteRecipes.RemoveRange(favoriteRecipes);

        //    // apagar autor
        //    _context.Authors.Remove(author);

        //    _context.SaveChanges();
        //}
    }
}
