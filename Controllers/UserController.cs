using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Utils? _utils;

        public UserController()
        {
            _dataQuery = new DataQuery();
            _utils = new Utils();
        }

        [HttpGet]
        [Route("GetUsers")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetUsers()
        {
            try
            {
                var rol = User.Claims
                              .Where(c => c.Type == "IdRol")
                              .Select(c => c.Value)
                              .FirstOrDefault();

                if (await _utils.HasRole(rol, "Administrador"))
                {
                    var response = await _dataQuery.GetUsers();

                    if (response is not null || response?.Count > 0)
                    {
                        var jsonString = JsonConvert.SerializeObject(response, Formatting.Indented);
                        return Ok(jsonString);
                    }
                    return NoContent();
                }
                return Unauthorized("No tienes permisos para acceder a este recurso");
            }
            catch
            {
                return BadRequest("Hubo un error en la consulta");
            }
        }

        [HttpPut]
        [Route("UpdateUsers")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateUsers([FromBody] User user)
        {
            try
            {
                if (user == null || user.id == 0)
                {
                    return BadRequest("Datos inválidos para la actualización");
                }

                user.login.password = _utils.HashGenerator(user.login.password);
                var updateResult = await _dataQuery.UpdateUser(user);

                if (updateResult > 0)
                {
                    return Ok("Actualización exitosa");
                }
                else
                {
                    return NotFound("No se encontró el usuario para actualizar");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error en la actualización");
            }
        }

        [HttpPost]
        [Route("PostUsers")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> PostUsers([FromBody] User user)
        {
            try
            {
                user.login.password = _utils.HashGenerator(user.login.password);
                var creationResult = await _dataQuery.InsertUser(user);

                if (creationResult > 0)
                {
                    return Ok("Usuario creado exitosamente");
                }
                else
                {
                    return BadRequest("No se pudo crear el usuario");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hubo un error en la creación del usuario: {ex.Message}");
            }
        }
    }
}