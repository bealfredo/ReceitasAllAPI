using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using ReceitasAllAPI.Persistence;

namespace ReceitasAllAPI.Controllers
{
    /// <summary>
    /// Controlador de autenticação
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor do controlador de autenticação
        /// </summary>
        /// <param name="context"></param>
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Rota para autenticação de autores (usuários da API)
        /// </summary>
        /// <param name="request"></param >
        /// <response code="200">Retorna o token de autenticação</response>
        /// <response code="401">Credenciais inválidas</response>
        /// <response code="500">Erro interno</response>
        /// <remarks>
        /// Este endpoint é responsável por autenticar um autor (usuário da API) e gerar um token JWT para ser utilizado nas requisições subsequentes.
        /// O token tem validade de 24 horas.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Valide as credenciais aqui

            // encontrando o usuário
            var author = _context.Authors.SingleOrDefault(a => a.UserName == request.Username);

            if (author == null)
            {
                return Unauthorized();
            }

            // Verifica a senha
            if (author.PasswordHash != request.Password)
            {
                return Unauthorized();
            }

            // Gera o token JWT
            var token = GenerateJwtToken(request.Username, author.ID.ToString(), author.Admin);

            // Adiciona o token no header da resposta
            Response.Headers.Append("Authorization", "Bearer " + token);

            return Ok(); // Retorna um código 200 OK sem corpo
            
        }

        private string GenerateJwtToken(string username, string userId, bool Admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("chave-secreta-super-segura-256bits");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("aud", "receitasallapi"),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, Admin ? "Admin" : "Author"),
                new Claim("userId", userId)
            }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = "localhost",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    /// <summary>
    /// Entidade de requisição de login
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

