using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;
using Microsoft.AspNetCore.Mvc;

namespace Clinica_SantaRosa.PL.WebApp.Controllers
{
    public class CitaController : Controller
    {
        private CitaBC _citaBC = new CitaBC();
        private PacienteBC _pacienteBC = new PacienteBC();
        private MedicoBC _medicoBC = new MedicoBC();
        private HorarioAtencionBC _horarioBC = new HorarioAtencionBC();

        public IActionResult Listar(string texto)
        {
            List<CitaBE> citas;

            // Lógica de búsqueda basada en el parámetro 'texto' del formulario
            if (string.IsNullOrEmpty(texto))
            {
                citas = _citaBC.ListarCitas();
            }
            else
            {
                citas = _citaBC.ListarCitasPorCriterio(texto);
            }

            // Pasamos 'texto' de vuelta a la vista para que el input no se limpie
            ViewBag.texto = texto;

            // Si usas TempData para mensajes de SweetAlert desde otras acciones (Guardar/Cancelar)
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];

            return View("ListadoCitas", citas);
        }

        [HttpGet]
        public IActionResult RegistrarCita()
        {
            // Enviamos una instancia vacía para que Razor sepa qué campos mapear
            CitaBE modelo = new CitaBE();
            modelo.Fecha_Cita = DateTime.Now;

            return View(modelo);
        }

        [HttpGet]
        public JsonResult BuscarPaciente(string filtro) => Json(_pacienteBC.buscarPacientePorFiltro(filtro));

        [HttpGet]
        public JsonResult BuscarMedico(string filtro) => Json(_medicoBC.BuscarMedicos(filtro));

        [HttpGet]
        public JsonResult ListarHorarios(long idMedico) {
            try
            {
                // Llama a ListarHorariosPorMedico que vimos en tu DALC
                var lista = _horarioBC.ListarHorarios(idMedico);
                return Json(lista);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult VerificarDisponibilidad(long idMedico, DateTime fecha)
        {
            try
            {
                // Llamada a la lógica que usa tu HorarioAtencionDALC
                var disponibilidad = _horarioBC.VerificarDisponibilidad(idMedico, fecha);
                return Json(disponibilidad);
            }
            catch (Exception ex)
            {
                // Enviamos un error 400 para que el JavaScript lo capture en el .catch()
                Response.StatusCode = 400;
                return Json(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] CitaBE obj)
        {
            try
            {
                long idGenerado = _citaBC.RegistrarCita(obj);
                return Ok(idGenerado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 1. Acción para mostrar la vista de edición cargando los datos de la cita
        [HttpGet]
        public IActionResult EditarCita(long id)
        {
            try
            {
                // Obtenemos la cita desde la Capa de Negocio
                CitaBE cita = _citaBC.ObtenerCitaPorId(id);

                if (cita == null)
                {
                    TempData["Error"] = "La cita solicitada no existe.";
                    return RedirectToAction("Listar");
                }

                // Retornamos la vista (debes crear EditarCita.cshtml) con el modelo cargado
                return View(cita);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la cita: " + ex.Message;
                return RedirectToAction("Listar");
            }
        }

        // 2. Acción para procesar la actualización de la cita (vía Fetch/AJAX)
        [HttpPost]
        public IActionResult Actualizar([FromBody] CitaBE obj)
        {
            try
            {
                // Ejecutamos la actualización en la Capa de Negocio
                bool actualizado = _citaBC.ActualizarCita(obj);

                // Retornamos un estado exitoso para que el JavaScript lo procese
                return Ok(actualizado);
            }
            catch (Exception ex)
            {
                // Enviamos el mensaje de error (incluyendo los de SQL) en formato JSON
                // Esto activará el .catch() en tu JavaScript
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult VerDetalle(long id)
        {
            try
            {
                // Reutilizamos la lógica de negocio que ya usa el SP USP_Cita_ObtenerPorId
                CitaBE cita = _citaBC.ObtenerCitaPorId(id);

                if (cita == null) return NotFound();

                // Devolvemos una vista parcial (solo el contenido del detalle)
                return PartialView("_DetalleCita", cita);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult Cancelar(long id)
        {
            try
            {
                bool cancelado = _citaBC.CancelarCita(id);
                if (cancelado)
                {
                    TempData["Success"] = "La cita ha sido cancelada y el horario está disponible nuevamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo realizar la cancelación.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Listar");
        }
    }
}
