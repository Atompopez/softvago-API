using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Utils? _utils;

        public ApiController()
        {
            _dataQuery = new DataQuery();
            _utils = new Utils();
        }

        [HttpGet]
        [Route("GetApis")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetApis()
        {
            try
            {
                var rol = User.Claims
                              .Where(c => c.Type == "IdRol")
                              .Select(c => c.Value)
                              .FirstOrDefault();

                if (await _utils.HasRole(rol, "Administrador"))
                {
                    var response = await _dataQuery.GetApis();

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
        [Route("UpdateApi")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateApi([FromBody] Api apiToUpdate)
        {
            try
            {
                var rol = User.Claims
                     .Where(c => c.Type == "IdRol")
                     .Select(c => c.Value).ToString();

                if (await _utils.HasRole(rol, "Administrador"))
                {
                    if (apiToUpdate == null || apiToUpdate.id == 0)
                    {
                        return BadRequest("Datos inválidos para la actualización");
                    }
                    var updateResult = await _dataQuery.UpdateApi(apiToUpdate);

                    if (updateResult > 0)
                    {
                        return Ok("Actualización exitosa");
                    }
                    else
                    {
                        return NotFound("No se encontró la API para actualizar");
                    }
                }
                return Unauthorized("No tienes permisos para acceder a este recurso");
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error en la actualización");
            }
        }
    }
}