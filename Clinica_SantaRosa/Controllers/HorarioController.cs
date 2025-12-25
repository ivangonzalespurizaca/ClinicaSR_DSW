using Clinica_SantaRosa.PL.WebApp.Models;
using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;
using Microsoft.AspNetCore.Mvc;

namespace Clinica_SantaRosa.PL.WebApp.Controllers
{
    [Route("Horarios")]
    public class HorarioController : Controller
    {
        private readonly HorarioAtencionBC _horarioBC = new HorarioAtencionBC();
        private readonly MedicoBC _medicoBC = new MedicoBC();

        // 1. Carga de la página principal
        [HttpGet("Gestionar")]
        public IActionResult Gestionar(long? idMedico)
        {
            var model = new GestionarHorariosViewModel();

            if (idMedico.HasValue && idMedico > 0)
            {
                model.Medico = _medicoBC.ObtenerPorId(idMedico.Value);
                model.ListaHorarios = _horarioBC.ListarHorarios(idMedico.Value);
            }
            else
            {
                model.ListaHorarios = new List<HorarioAtencionBE>();
            }

            // Especificamos el nombre exacto del archivo .cshtml
            return View("GestionarHorarios", model);
        }

        // 2. Búsqueda AJAX para el Modal
        [HttpGet("BuscarMedico")]
        public JsonResult BuscarMedico(string filtro)
        {
            var medicos = _medicoBC.BuscarMedicos(filtro);
            return Json(medicos);
        }

        // 3. Registro (Post-Redirect-Get Pattern)
        [HttpPost("Guardar")]
        public IActionResult Guardar([Bind(Prefix = "NuevoHorario")] HorarioAtencionBE entidad)
        {
            string mensaje;

            if (entidad.MedicoBE == null) entidad.MedicoBE = new MedicoBE();

            bool exito = _horarioBC.RegistrarHorario(entidad, out mensaje);

            if (exito)
            {
                TempData["Success"] = "Horario registrado correctamente";
            }
            else
            {
                TempData["Error"] = mensaje;
            }

            // Redirigimos manteniendo el contexto del médico seleccionado
            return RedirectToAction("Gestionar", new { idMedico = entidad.MedicoBE.ID_Medico });
        }

        // 4. Eliminación limpia
        [HttpPost("Eliminar")]
        public IActionResult Eliminar(long idHorario, long idMedico)
        {
            string mensaje;
            _horarioBC.EliminarHorario(idHorario, out mensaje);

            TempData["Success"] = mensaje;
            return RedirectToAction("Gestionar", new { idMedico = idMedico });
        }
    }
}
