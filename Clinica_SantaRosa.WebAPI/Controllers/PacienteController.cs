using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;
using Microsoft.AspNetCore.Mvc;

namespace Clinica_SantaRosa.WebAPI.Controllers
{
    [ApiController]
    [Route("ruta/[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly PacienteBC pacienteBC;

        public PacienteController()
        {
            pacienteBC = new PacienteBC();
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var lista = pacienteBC.ListarPacientes();
                return Ok(lista);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }   

        [HttpGet("{id}")] 
        public IActionResult ObtenerPorId(int id)
        {
            try
            {
                var paciente = pacienteBC.BuscarPacientePorId(id);
                if (paciente == null)
                {
                    return NotFound($"Paciente con ID {id} no encontrado.");
                }
                return Ok(paciente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpDelete("eliminar/{id}")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                bool eliminado = pacienteBC.EliminarPaciente(id);
                if (!eliminado)
                {
                    return NotFound($"Paciente con ID {id} no encontrado para eliminar.");
                }
                return Ok($"Paciente con ID {id} eliminado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("insertar")]
        public IActionResult Insertar([FromBody] PacienteBE pacienteBE)
        {
            try
            {
                var pacienteInsertado = pacienteBC.RegistrarPaciente(pacienteBE);

                return Ok(new
                {
                    ID = pacienteInsertado.ID_Paciente,
                    Mensaje = "Paciente insertado correctamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] PacienteBE pacienteBE)
        {
            if (pacienteBE.ID_Paciente <= 0)
                return BadRequest("ID_Paciente es obligatorio");

            bool actualizado = pacienteBC.ActualizarPaciente(pacienteBE);

            if (!actualizado)
                return NotFound("Paciente no encontrado");

            return Ok("PACIENTE ACTUALIZADO");
        }




    }
}
