using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceitasAllAPI.Dtos;
using ReceitasAllAPI.Entities;
using ReceitasAllAPI.Persistence;

namespace ReceitasAllAPI.Controllers
{
    /// <summary>
    /// Controlador de autores - usuários da API
    /// </summary>
    [Authorize]
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor do controlador de autores
        /// </summary>
        /// <param name="context"></param>
        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um conta de autor (usuário do sistema).
        /// </summary>
        /// <param name="authorDto">Objeto contendo os dados do autor a ser criado.</param>
        /// <returns>Os detalhes do autor recém-criado.</returns>
        /// <response code="201">Autor criado com sucesso.</response>
        /// <response code="400">A requisição é inválida. Possíveis razões incluem:
        /// Os dados fornecidos são inválidos.
        /// </response>
        /// <response code="409">Já existe um autor com o mesmo UserName informado.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint permite a criação de um novo autor no sistema.
        /// 
        /// É necessário que os dados fornecidos estejam válidos e que o UserName seja único.
        /// 
        /// O username e a senha serão utilizados para autenticação do autor.
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

            return StatusCode(201, authorResponse);
        }

        /// <summary>
        /// Recupera o perfil do autor autenticado.
        /// </summary>
        /// <returns>Os detalhes do autor autenticado.</returns>
        /// <response code="200">O perfil do autor autenticado foi retornado com sucesso.</response>
        /// <response code="401">O usuário não está autenticado.</response>
        /// <response code="404">O autor autenticado não foi encontrado.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint permite que um autor autenticado recupere as informações do seu próprio perfil.
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Atualiza o perfil do autor autenticado.
        /// </summary>
        /// <param name="authorDto">Objeto contendo os novos dados do autor.</param>
        /// <returns>Status indicando o resultado da operação.</returns>
        /// <response code="204">O perfil foi atualizado com sucesso.</response>
        /// <response code="401">O usuário não está autenticado.</response>
        /// <response code="404">O autor autenticado não foi encontrado.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint permite que o autor autenticado atualize as informações do seu perfil.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Atualiza a senha do autor autenticado.
        /// </summary>
        /// <param name="authorDto">Objeto contendo a senha antiga e a nova senha do autor.</param>
        /// <returns>Status indicando o resultado da operação.</returns>
        /// <response code="204">A senha foi atualizada com sucesso.</response>
        /// <response code="400">A senha antiga fornecida está incorreta.</response>
        /// <response code="401">O usuário não está autenticado.</response>
        /// <response code="404">O autor autenticado não foi encontrado.</response>
        /// <response code="500">Erro interno ao processar a solicitação.</response>
        /// <remarks>
        /// Este endpoint permite que o autor autenticado atualize sua senha.
        /// 
        /// É necessário fornecer a senha antiga corretamente para realizar a alteração.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("myprofile/edit/password")]
        public IActionResult UpdatePassword(AuthorUpdatePasswordDto authorDto)
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

            if (authorEntity.PasswordHash != authorDto.OldPassword)
            {
                return BadRequest("Senha antiga incorreta.");
            }

            authorEntity.PasswordHash = authorDto.NewPassword;

            _context.Authors.Update(authorEntity);
            _context.SaveChanges();

            return NoContent();
        }

        // por enquanto não porque o usuario pode ser apagado e o token ainda funcionar
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
