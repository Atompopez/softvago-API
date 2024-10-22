using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;

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