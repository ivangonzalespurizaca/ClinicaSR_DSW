using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Clinica_SantaRosa.PL.WebApp.Controllers
{
    public class UsuariosController : Controller
    {
        private UsuarioBC _usuarioBC = new UsuarioBC();
        private readonly IWebHostEnvironment _env;
        public UsuariosController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpGet]
        public IActionResult RegistrarUsuario()
        {
            // Enviamos un objeto vacío para inicializar los Tag Helpers de la vista
            return View(new UsuarioBE());
        }
        [HttpPost]
        public async Task<IActionResult> Registro(UsuarioBE usuario, IFormFile archivoImagen)
        {
            try
            {
                // 1. MANEJO DE IMAGEN DE PERFIL
                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    // Definimos la carpeta de destino: wwwroot/images
                    string folder = Path.Combine(_env.WebRootPath, "images");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    // Generamos un nombre único para evitar colisiones
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await archivoImagen.CopyToAsync(stream);
                    }

                    // Guardamos la ruta relativa en la base de datos
                    usuario.Img_Perfil = "/images/" + fileName;
                }

                // 2. REGISTRO EN CAPA DE NEGOCIO (Aquí se valida DNI y Username)
                _usuarioBC.RegistrarUsuario(usuario);

                TempData["Success"] = $"El usuario {usuario.Username} ha sido registrado correctamente.";
                return RedirectToAction("PanelAdministrador", "Home");
            }
            catch (Exception ex)
            {
                // Si la BC lanza una excepción (ej: DNI duplicado), lo capturamos aquí
                ViewBag.Error = ex.Message;
                return View("RegistrarUsuario", usuario);
            }
        }
        // GET: Usuarios/Editar?nombreBusqueda=admin
        [HttpGet]
        public IActionResult EditarUsuario(string nombreBusqueda)
        {
            UsuarioBE usuarioEncontrado = null;

            if (!string.IsNullOrEmpty(nombreBusqueda))
            {
                try
                {
                    usuarioEncontrado = _usuarioBC.BuscarPorUserName(nombreBusqueda);
                    // Limpiamos la contraseña para que no viaje a la vista
                    if (usuarioEncontrado != null) usuarioEncontrado.Contrasenia = "";
                }
                catch
                {
                    ViewBag.Error = "Usuario no encontrado.";
                }
            }

            return View(usuarioEncontrado);
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(UsuarioBE usuario, IFormFile archivoImagen)
        {
            try
            {
                // 1. Obtener el ID del usuario logueado actualmente (desde la sesión o claims)
                // Supongamos que guardaste el ID en un Claim al hacer login
                string idEnSesion = HttpContext.Session.GetString("UsuarioID");

                // Validación de seguridad por si la sesión expiró
                if (string.IsNullOrEmpty(idEnSesion))
                {
                    return RedirectToAction("Login", "Seguridad");
                }

                long idUsuarioLogueado = long.Parse(idEnSesion);

                // 2. Procesar imagen (tu lógica actual...)
                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    string folder = Path.Combine(_env.WebRootPath, "images");
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await archivoImagen.CopyToAsync(stream);
                    }
                    usuario.Img_Perfil = "/images/" + fileName;
                }

                // 3. Ejecutar la actualización
                bool exito = _usuarioBC.ActualizarUsuario(usuario);

                if (usuario.ID_Usuario == idUsuarioLogueado)
                {
                    HttpContext.Session.Clear(); // Limpiamos la sesión
                    TempData["Success"] = "Datos actualizados. Inicie sesión de nuevo.";
                    return RedirectToAction("Login", "Seguridad");
                }

                return RedirectToAction("PanelAdministrador", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View("EditarUsuario", usuario);
        }
    }
}
