using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Utils? _utils;

        public ClientController()
        {
            _dataQuery = new DataQuery();
            _utils = new Utils();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] Login loginCredentials)
        {
            try
            {
                var response = await _dataQuery.Authenticate(loginCredentials);

                if (response.success)
                {
                    var token = _utils.GenerateJwtToken(loginCredentials, response.idRol);
                    var responseAPI = new
                    {
                        Message = "Inicio de sesión exitoso",
                        data = response.data,
                        Token = token
                    };

                    return Ok(responseAPI);
                }

                return Unauthorized("Credenciales incorrectas");
            }
            catch
            {
                return BadRequest("Hubo un error durante el proceso de login");
            }
        }
    }
}