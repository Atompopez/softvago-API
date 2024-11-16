using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Utils? _utils;
        private readonly ApiService? _apiService;

        public JobController()
        {
            _dataQuery = new DataQuery();
            _utils = new Utils();
            _apiService = new ApiService();
        }

        [HttpPost]
        [Route("PostJobs")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> PostJobs([FromBody] JobSearchParameters searchParameters)
        {
            try
            {
                var rol = User.Claims
                              .Where(c => c.Type == "IdRol")
                              .Select(c => c.Value)
                              .FirstOrDefault();

                bool Admin = await _utils.HasRole(rol, "Administrador");

                await _apiService.SearchJobsAsync(searchParameters);

                var response = await _dataQuery.GetJobs(searchParameters, Admin);

                if (response is not null && response?.Count > 0)
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> UpdateJob([FromBody] Job jobToUpdate)
        {
            try
            {
                var rol = User.Claims
                              .Where(c => c.Type == "IdRol")
                              .Select(c => c.Value)
                              .FirstOrDefault();

                if (await _utils.HasRole(rol, "Administrador"))
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
                return Unauthorized("No tienes permisos para acceder a este recurso");
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error en la actualización");
            }
        }

        [HttpPut]
        [Route("AddClickJob/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> AddClickJob(int id)
        {
            try
            {
                var rol = User.Claims
                              .Where(c => c.Type == "IdRol")
                              .Select(c => c.Value)
                              .FirstOrDefault();

                if (await _utils.HasRole(rol, "Usuario") || await _utils.HasRole(rol, "Invitado"))
                {
                    if (id == 0)
                    {
                        return BadRequest("Datos inválidos para la actualización");
                    }
                    var updateResult = await _dataQuery.AddClickJob(id);

                    if (updateResult > 0)
                    {
                        return Ok("Actualización exitosa");
                    }
                    else
                    {
                        return NotFound("No se encontró el empleo para actualizar");
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