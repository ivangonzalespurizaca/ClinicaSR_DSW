using ClinicaSR.BL.BE;
using ClinicaSR.DL.DALC;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BC
{
    public class UsuarioBC
    {
        private UsuarioDALC usuarioDALC = new UsuarioDALC();
        public List<UsuarioBE> ListarUsuarios()
        {
            return usuarioDALC.listarUsuarios();
        }

        public UsuarioBE Login(string username, string password)
        {
            return usuarioDALC.Login(username, password);
        }
    }
}
