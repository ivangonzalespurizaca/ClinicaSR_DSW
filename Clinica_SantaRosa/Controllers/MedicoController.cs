using Clinica_SantaRosa.PL.WebApp.Models;
using ClinicaSR.BL.BE;
using ClinicaSR.DL.DALC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Clinica_SantaRosa.PL.WebApp.Controllers
{
    public class MedicoController : Controller
    {
        private readonly MedicoModel _model;
        private readonly EspecialidadDALC _especialidadDALC;

        public MedicoController()
        {
            _model = new MedicoModel();
            _especialidadDALC = new EspecialidadDALC();
        }

        public IActionResult IndexMedico()
        {
            return View(_model.listado());
        }

        [HttpGet]
        public IActionResult Registro()
        {
            var medico = new MedicoBE
            {
                EspecialidadBE = new EspecialidadBE()
            };

            ViewData["Especialidades"] = _especialidadDALC.ListarEspecialidades();
            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(MedicoBE medico)
        {
            if (medico.EspecialidadBE == null)
                medico.EspecialidadBE = new EspecialidadBE();

            if (ModelState.IsValid)
            {
                int idGenerado = _model.insertar(medico);

                if (idGenerado > 0)
                {
                    TempData["Success"] = "Médico registrado correctamente"; // ✅ Mensaje de éxito
                    return RedirectToAction("IndexMedico");
                }

                TempData["Error"] = "Error al insertar el médico"; // ✅ Mensaje de error
            }

            ViewData["Especialidades"] = _especialidadDALC.ListarEspecialidades();
            return View(medico);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var medico = _model.obtenerxid(id);

            if (medico == null)
            {
                TempData["Error"] = "No se encontró el médico solicitado";
                return RedirectToAction("IndexMedico");
            }

            var especialidades = _especialidadDALC.ListarEspecialidades();
            ViewBag.Especialidades = new SelectList(especialidades, "ID_Especialidad", "Nombre", medico.EspecialidadBE.ID_Especialidad);

            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(MedicoBE medico)
        {
            if (ModelState.IsValid)
            {
                bool actualizado = _model.actualizar(medico);

                if (actualizado)
                {
                    TempData["Success"] = "Médico actualizado correctamente"; // ✅ Mensaje de éxito
                    return RedirectToAction("IndexMedico");
                }

                TempData["Error"] = "Error al actualizar el médico"; // ✅ Mensaje de error
            }

            ViewData["Especialidades"] = _especialidadDALC.ListarEspecialidades();
            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            bool eliminado = _model.eliminar(id);

            if (eliminado)
            {
                TempData["Success"] = "Médico eliminado correctamente"; // ✅ Mensaje de éxito
                return RedirectToAction("IndexMedico");
            }

            TempData["Error"] = "No se puede eliminar el médico porque tiene citas registradas"; // ✅ Mensaje de error
            return RedirectToAction("IndexMedico");
        }
    }
}
