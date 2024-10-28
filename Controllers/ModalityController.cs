using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModalityController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Utils? _utils;

        public ModalityController()
        {
            _dataQuery = new DataQuery();
            _utils = new Utils();
        }

        [HttpGet]
        [Route("GetModality")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetModality()
        {
            try
            {
                var response = await _dataQuery.GetModality();

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
        [Route("UpdateModality")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateModality([FromBody] Modality modalityToUpdate)
        {
            try
            {
                if (modalityToUpdate == null || modalityToUpdate.id == 0)
                {
                    return BadRequest("Datos inválidos para la actualización");
                }
                var updateResult = await _dataQuery.UpdateModality(modalityToUpdate);

                if (updateResult > 0)
                {
                    return Ok("Actualización exitosa");
                }
                else
                {
                    return NotFound("No se encontró la modalidad para actualizar");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error en la actualización");
            }
        }
    }
}