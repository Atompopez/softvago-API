using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Utils? _utils;

        public LocationsController()
        {
            _dataQuery = new DataQuery();
            _utils = new Utils();
        }

        [HttpGet]
        [Route("GetLocations")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetLocations()
        {
            try
            {
                var response = await _dataQuery.GetLocations();

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
        [Route("UpdateLocation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateLocation([FromBody] Location locationToUpdate)
        {
            try
            {
                if (locationToUpdate == null || locationToUpdate.id == 0)
                {
                    return BadRequest("Datos inválidos para la actualización");
                }
                var updateResult = await _dataQuery.UpdateLocation(locationToUpdate);

                if (updateResult > 0)
                {
                    return Ok("Actualización exitosa");
                }
                else
                {
                    return NotFound("No se encontró la localización para actualizar");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error en la actualización");
            }
        }
    }
}