using ClinicaSR.BL.BC;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Clinica_SantaRosa.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicoController : ControllerBase
    {
        private readonly MedicoBC medicoBC;

        public MedicoController()
        {
            medicoBC = new MedicoBC();
        }

        // GET: api/<MedicoController>
        [HttpGet]
        public IActionResult Listar()
        {
            return Ok(medicoBC.ListarMedicos());
        }

      
        // GET ruta/Medico?nombre=Juan
        [HttpGet("buscar")]
        public IActionResult GetPorNombre([FromQuery] string nombre)
        {
            var medico = medicoBC.BuscarMedicos(nombre);
            return Ok(medico);
        }


        // POST api/<MedicoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MedicoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MedicoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
