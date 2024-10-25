using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using softvago_API.Logica;
using softvago_API.Models;

namespace softvago_API.Controllers
{
    public class ClientController : Controller
    {
        private readonly DataQuery? _dataQuery;
        private readonly Utils? _utils;

        public ClientController()
        {
            _dataQuery = new DataQuery();
            _utils = new Utils();
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
                    var token = _utils.GenerateJwtToken(loginCredentials);
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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

        

        [HttpGet]
        [Route("GetRoles")]
        [Authorize(AuthenticationSchemes = "Bearer")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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
        [Authorize(AuthenticationSchemes = "Bearer")]
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

        [HttpGet]
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
        //[Authorize(AuthenticationSchemes = "Bearer")]
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