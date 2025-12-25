using Clinica_SantaRosa.PL.WebApp.Models;
using ClinicaSR.BL.BE;
using Microsoft.AspNetCore.Mvc;

namespace Clinica_SantaRosa.PL.WebApp.Controllers
{
    public class SeguridadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListaUsuarios()
        {
            return View();
        }

        public IActionResult Login()
        {
            // Verificar si ya existe el ID de usuario en la sesión
            if (HttpContext.Session.GetString("UsuarioID") != null)
            {
                string rol = HttpContext.Session.GetString("UsuarioRol");

                // Redirigir según el rol detectado en la sesión activa
                if (rol == "ADMINISTRADOR") return RedirectToAction("PanelAdministrador", "Home");
                if (rol == "RECEPCIONISTA") return RedirectToAction("PanelRecepcionista", "Home");
                if (rol == "CAJERO") return RedirectToAction("PanelCajero", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string contrasenia)
        {
            UsuarioModel model = new UsuarioModel();
            var usuario = model.Login(username, contrasenia);

            if (usuario != null)
            {
                // GUARDAR DATOS EN SESIÓN (Crucial para el Middleware)
                HttpContext.Session.SetString("UsuarioID", usuario.ID_Usuario.ToString());
                HttpContext.Session.SetString("UsuarioRol", usuario.Rol.ToString());
                HttpContext.Session.SetString("NombreCompleto", $"{usuario.Nombres} {usuario.Apellidos}");
                // Si la imagen es nula en la BD, guardamos una cadena vacía o una ruta por defecto
                string rutaFoto = string.IsNullOrEmpty(usuario.Img_Perfil)
                                  ? "/images/default-user.jpg"
                                  : usuario.Img_Perfil;
                HttpContext.Session.SetString("FotoPerfil", rutaFoto);
                // REDIRECCIÓN SEGÚN ROL
                if (usuario.Rol == Rol.ADMINISTRADOR)
                {
                    return RedirectToAction("PanelAdministrador", "Home");
                }
                else if (usuario.Rol == Rol.RECEPCIONISTA)
                {
                    return RedirectToAction("PanelRecepcionista", "Home");
                }
                else if (usuario.Rol == Rol.CAJERO)
                {
                    return RedirectToAction("PanelCajero", "Home");
                }

                return RedirectToAction("Index", "Seguridad");
            }
            else
            {
                // Podrías usar TempData para enviar un mensaje de error a la vista
                TempData["ErrorLogin"] = "Credenciales incorrectas.";
                return RedirectToAction("Login", "Seguridad");
            }
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Seguridad");
        }
    }
}
