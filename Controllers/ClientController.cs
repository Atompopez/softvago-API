using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    public class ClientController : Controller
    {
        private readonly DataQuery? _dataQuery;
        public ClientController()
        {
            _dataQuery = new DataQuery();
        }

        [HttpGet]
        [Route("GetApis")]
        public async Task<ActionResult> GetApis()
        {
            try
            {
                var response = await _dataQuery.GetApis();

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
        [Route("UpdateApi")]
        public async Task<ActionResult> UpdateApi([FromBody] Api apiToUpdate)
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest("Hubo un error en la actualización");
            }
        }

        [HttpGet]
        [Route("GetJobs")]
        public async Task<ActionResult> GetJobs()
        {
            try
            {
                var response = await _dataQuery.GetJobs();

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
        [Route("UpdateJob")]
        public async Task<ActionResult> UpdateJob([FromBody] Job jobToUpdate)
        {
            try
            {
                if (jobToUpdate == null || jobToUpdate.id == 0)
                {
                    return BadRequest("Datos inválidos para la actualización");
                }
                var updateResult = await _dataQuery.UpdateJob(jobToUpdate);

                if (updateResult > 0)
                {
                    return Ok("Actualización exitosa");
                }
                else
                {
                    return NotFound("No se encontró el empleo para actualizar");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error en la actualización");
            }
        }

        [HttpGet]
        [Route("GetModality")]
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
        [Route("UpdateJob")]
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

        [HttpGet]
        [Route("GetLocations")]
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

        [HttpGet]
        [Route("GetRoles")]
        public async Task<ActionResult> GetRoles()
        {
            try
            {
                var response = await _dataQuery.GetRoles();

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

        [HttpGet]
        [Route("GetUsers")]
        public async Task<ActionResult> GetUsers()
        {
            try
            {
                var response = await _dataQuery.GetUsers();

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
    }
}