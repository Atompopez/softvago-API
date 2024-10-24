using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    public class ClientController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Jwt? _jwt;

        public ClientController()
        {
            _dataQuery = new DataQuery();
            _jwt = new Jwt();
        }

        [HttpGet]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] Login loginCredentials)
        {
            try
            {
                var response = await _dataQuery.Authenticate(loginCredentials);

                if (response.success)
                {
                    var token = GenerateJwtToken(user);
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

        [HttpPut]
        [Route("AddClickJob/{id}")]
        public async Task<ActionResult> AddClickJob([FromBody] int id) 
        {
            try
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

        [HttpPut]
        [Route("UpdateLocation")]
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
        [Route("UpdateModality")]
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

        [HttpPut]
        [Route("UpdateRole")]
        public async Task<ActionResult> UpdateRole([FromBody] Rol roleToUpdate)
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest("Hubo un error en la actualización");
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