using Clinica_SantaRosa.PL.WebApp.Models;
using ClinicaSR.BL.BE;
using Microsoft.AspNetCore.Mvc;

namespace Clinica_SantaRosa.PL.WebApp.Controllers
{
    public class PacienteController : Controller
    {
        private readonly PacienteModel _model;

        public PacienteController()
        {
            _model = new PacienteModel();
        }

        public IActionResult Listado()
        {
            return View(_model.lista());
        }

        // ===================== REGISTRO =====================

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(PacienteBE paciente)
        {
            if (!ModelState.IsValid)
                return View(paciente);

            PacienteBE pacienteRegistrado = _model.RegistrarPaciente(paciente);

            if (pacienteRegistrado == null)
            {
                TempData["Error"] = "No se pudo registrar el paciente";
                return View(paciente);
            }

            TempData["Success"] = "Paciente registrado correctamente";
            return RedirectToAction("Listado");
        }


        // ===================== EDITAR =====================

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var paciente = _model.ObtenerPorId(id);
            if (paciente == null)
                return NotFound();

            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(PacienteBE paciente)
        {
            if (!ModelState.IsValid)
                return View(paciente);

            bool actualizado = _model.Actualizar(paciente);

            if (!actualizado)
            {
                TempData["Error"] = "No se pudo actualizar el paciente";
                return View(paciente);
            }

            TempData["Success"] = "Paciente actualizado correctamente";
            return RedirectToAction("Listado");
        }

        // ===================== ELIMINAR =====================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            bool eliminado = _model.Eliminar(id);

            if (!eliminado)
            {
                TempData["Error"] = "No se puede eliminar el paciente porque tiene citas registradas";
                return RedirectToAction("Listado");
            }

            TempData["Success"] = "Paciente eliminado correctamente";
            return RedirectToAction("Listado");
        }
    }
}
