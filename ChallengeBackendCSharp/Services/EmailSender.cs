using ChallengeBackendCSharp.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ChallengeBackendCSharp.Services
{
    public class EmailSender
    {
        public readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        ///     Envía un email dando la bienvenida cuando el registro es completado correctamente.
        /// </summary>
        /// <param name="registerUser">Se debe pasar como parámetro un objeto RegisterModel.</param>
        /// <returns>True: si el envió fue completado correctamente, False: si no se pudo enviar el email.</returns>
        public async Task<bool> SendWelcomeEmail(RegisterModel registerUser)
        {
            var client = new SendGridClient(_config["SendGrid:API_KEY"]);
            
            var from = new EmailAddress("mt2.shopping.box@outlook.com", "Disney Web API");
            var subject = "Bienvenido a Disney Web API!, su registro se completo correctamente.";
            var to = new EmailAddress(registerUser.Email, registerUser.Username);

            var plainTextContent = $"Bienvenido {registerUser.Username}!, su registro ya fue procesado, y esta listo para utilizar Disney Web API.";
            var htmlContent = string.Format("<body style=\"font-family: sans-serif; background-color: #f9f9f9;\"> <div style=\"background-color: white; border: 1px solid lightgray; border-radius: 10px; box-shadow: 2px 2px 2px 1px rgba(0, 0, 0, 0.065); margin: 15px;\"> <div style=\"margin: 10px;\"> <h4>Hola {0}!</h4> <p>Bienvenido a <strong>Disney Web API</strong>, su registro se completo correctamente y ya pude empezar a utilizar el servicio.</p> <p style=\"margin-top: 40px;\">Saludos.</p> </div> </div></body>",
                                     registerUser.Username);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
    }
}
