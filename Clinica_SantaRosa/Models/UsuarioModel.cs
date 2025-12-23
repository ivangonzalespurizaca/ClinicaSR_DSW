using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;

namespace Clinica_SantaRosa.PL.WebApp.Models
{
    public class UsuarioModel
    {
        private UsuarioBC usuarioBC = new UsuarioBC();

        public List<UsuarioBE> listaUsuarios()
        {
            return usuarioBC.ListarUsuarios();
        }
        public UsuarioBE Login(string username, string password)
        {
            return usuarioBC.Login(username, password);
        }

    }
}
