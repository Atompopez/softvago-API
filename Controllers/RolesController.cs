using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Utils? _utils;

        public RolesController()
        {
            _dataQuery = new DataQuery();
            _utils = new Utils();
        }

        [HttpGet]
        [Route("GetRoles")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetRoles()
        {
            try
            {
                var rol = User.Claims
                              .Where(c => c.Type == "IdRol")
                              .Select(c => c.Value)
                              .FirstOrDefault();

                bool Admin = await _utils.HasRole(rol, "Administrador");

                var response = await _dataQuery.GetRoles(Admin);

                if (response is not null || response?.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(response, Formatting.Indented);
                    return Ok(jsonString);
                }
                return NoContent();
            }
            catch
            {
                return BadRequest("Hubo un error en la consulta");
            }
        }

        [HttpPut]
        [Route("UpdateRole")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateRole([FromBody] Rol roleToUpdate)
        {
            try
            {
                var rol = User.Claims
                              .Where(c => c.Type == "IdRol")
                              .Select(c => c.Value)
                              .FirstOrDefault();

                if (await _utils.HasRole(rol, "Administrador"))
                {
                    if (roleToUpdate == null || roleToUpdate.id == 0)
                    {
                        return BadRequest("Datos inválidos para la actualización");
                    }
                    var updateResult = await _dataQuery.UpdateRol(roleToUpdate);

                    if (updateResult > 0)
                    {
                        return Ok("Actualización exitosa");
                    }
                    else
                    {
                        return NotFound("No se encontró el rol para actualizar");
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