using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;
using Microsoft.AspNetCore.Mvc;

namespace Clinica_SantaRosa.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitaController : Controller
    {
        private readonly CitaBC citaBC;
        public CitaController()
        {
            citaBC = new CitaBC();
        }
        [HttpGet]
        public IActionResult Listar(string? criterio = null)
        {
            try
            {
                var lista = string.IsNullOrWhiteSpace(criterio)
                            ? citaBC.ListarCitas()
                            : citaBC.ListarCitasPorCriterio(criterio);

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }
}
