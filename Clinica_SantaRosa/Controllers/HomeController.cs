using Microsoft.AspNetCore.Mvc;

namespace ClinicaSR.PL.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult PanelAdministrador()
        {
            return View();
        }
        public IActionResult PanelRecepcionista()
        {
            return View();
        }
        public IActionResult PanelCajero()
        {
            return View();
        }
    }
}
