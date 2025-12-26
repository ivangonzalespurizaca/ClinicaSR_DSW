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

            // Validación de especialidad
            if (medico.EspecialidadBE.ID_Especialidad == 0)
            {
                TempData["Alert"] = "error|Error|Debe seleccionar una especialidad";
                ViewBag.Especialidades = _especialidadDALC.ListarEspecialidades();
                return View(medico);
            }

            // Validación DNI único
            if (_model.ExisteDNI(medico.DNI))
            {
                TempData["Alert"] = "error|Error|El DNI ingresado ya esta registrado";
                ViewBag.Especialidades = _especialidadDALC.ListarEspecialidades();
                return View(medico);
            }

            // Validación Nro. Colegiatura único
            if (_model.ExisteNroColegiatura(medico.Nro_Colegiatura))
            {
                TempData["Alert"] = "error|Error|El Nro. de colegiatura ya esta registrado";
                ViewBag.Especialidades = _especialidadDALC.ListarEspecialidades();
                return View(medico);
            }

            // Insertar
            medico.ID_EspecialidadSeleccionada = medico.EspecialidadBE.ID_Especialidad;
            int idGenerado = _model.insertar(medico);

            if (idGenerado > 0)
                TempData["Success"] = "¡Éxito!|Médico registrado correctamente";
            else
                TempData["Alert"] = "error|Error|No se pudo registrar el médico";

            return RedirectToAction("IndexMedico");
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

            medico.ID_EspecialidadSeleccionada = medico.EspecialidadBE?.ID_Especialidad ?? 0;
            ViewBag.Especialidades = new SelectList(_especialidadDALC.ListarEspecialidades(), "ID_Especialidad", "Nombre", medico.ID_EspecialidadSeleccionada);
            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(MedicoBE medico)
        {
            medico.EspecialidadBE ??= new EspecialidadBE();
            medico.EspecialidadBE.ID_Especialidad = medico.ID_EspecialidadSeleccionada;
           
            
                bool actualizado = _model.actualizar(medico);
                if (actualizado)
                {
                    TempData["Success"] = "Médico actualizado correctamente";
                    return RedirectToAction("IndexMedico");
                }
                TempData["Error"] = "Error al actualizar el médico";
            

            ViewBag.Especialidades = new SelectList(_especialidadDALC.ListarEspecialidades(), "ID_Especialidad", "Nombre", medico.ID_EspecialidadSeleccionada);
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
