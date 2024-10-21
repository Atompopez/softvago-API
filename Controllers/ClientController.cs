using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [Route("GetJobs")]
        public async Task<ActionResult> GetJobs()
        {
            try
            {
                var response = await _dataQuery.GetJobs();
                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }  
    }
}