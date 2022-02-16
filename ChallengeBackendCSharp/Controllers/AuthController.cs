using ChallengeBackendCSharp.Models;
using ChallengeBackendCSharp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChallengeBackendCSharp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly EmailSender _emailSender;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration, EmailSender emailSender)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     Registra al usuario mediante una solicitud POST, los datos de registro deben ser enviados en el BODY.
        /// </summary>
        /// <param name="register">Datos necesarios: Nombre de usuario, Email y Contraseña</param>
        /// <returns>200: Si la solicitud pudo procesarse correctamente, o 404 si se produjo alguna excepción.</returns>
        [HttpPost("register")]
        public async Task<ActionResult> PostRegister([FromBody] RegisterModel register)
        {
            IdentityResult result = null!;

            try
            {
                var user = new IdentityUser { UserName = register.Username, Email = register.Email };

                // Crea el usuario y lo guarda en la base de datos.
                result = await _userManager.CreateAsync(user, register.Password);

                if (!result.Succeeded) throw new Exception("No se ha podido registrar el usuario en la base de datos.");

                // Envía el Mail de Bienvenida
                var statusMail = await _emailSender.SendWelcomeEmail(register);

                return Ok(new { Message = "El usuario fue agregado exitosamente a la base de datos.", EmailSent = statusMail });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message, result.Errors });
            }
        }

        /// <summary>
        ///     Permite a los usuarios ya registrados obtener su Token (JWT) para utilizar todas las funciones restringidas de la API.
        /// </summary>
        /// <param name="login">Datos necesarios: Nombre de usuario y Contraseña</param>
        /// <returns>200: Si la solicitud pudo procesarse correctamente junto con el Token (JWT), o 404 si se produjo alguna excepción.</returns>
        [HttpPost("login")]
        public async Task<ActionResult> PostLogin([FromBody] LoginModel login)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(login.Username);

                // Si el usuario no es encontrado, o la contraseña no es valida, devuelve un error 401: No autorizado.
                if (user is null | !await _userManager.CheckPasswordAsync(user!, login.Password)) return Unauthorized(new { Message = "El usuario no existe, o la contraseña ingresada no es correcta." });

                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, login.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _configuration["JSONWebToken:Issuer"],
                    audience: _configuration["JSONWebToken:Audience"],
                    claims: authClaims,
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JSONWebToken:Key"])), SecurityAlgorithms.HmacSha256)
                    );

                // Retorna el token junto a su fecha de vencimiento.
                return Ok(new { Message = "El token se creo exitosamente.", Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = token.ValidTo.ToString("dd-MM-yy HH:mm") });
            }
            catch (Exception ex)
            {
                return NotFound(new { ex.Message });
            }
        }
    }
}
