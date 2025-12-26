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

        public UsuarioBE Login(string username, string password)
        {
            return usuarioDALC.Login(username, password);
        }
        public bool RegistrarUsuario(UsuarioBE obj)
        {
            // 1. Validar que el Username no exista
            if (usuarioDALC.ObtenerPorUsername(obj.Username) != null)
                throw new Exception($"El nombre de usuario {obj.Username} ya esta en uso.");

            // 2. Validar que el DNI no exista
            if (usuarioDALC.ObtenerPorDni(obj.DNI) != null)
                throw new Exception($"El DNI {obj.DNI} ya esta registrado con otro usuario.");

            // 3. Proceder al registro (El SP se encarga del HASH SHA2_512)
            return usuarioDALC.Registrar(obj);
        }

        public bool ActualizarUsuario(UsuarioBE obj)
        {
            // 1. Validar unicidad del Username (Excepto él mismo)
            UsuarioBE userConEseNombre = usuarioDALC.ObtenerPorUsername(obj.Username);
            if (userConEseNombre != null && userConEseNombre.ID_Usuario != obj.ID_Usuario)
            {
                throw new Exception("El nombre de usuario ya pertenece a otra persona.");
            }

            // 2. Validar unicidad del DNI (Excepto él mismo)
            UsuarioBE userConEseDni = usuarioDALC.ObtenerPorDni(obj.DNI);
            if (userConEseDni != null && userConEseDni.ID_Usuario != obj.ID_Usuario)
            {
                throw new Exception("El DNI ingresado ya esta asignado a otro usuario.");
            }

            if (string.IsNullOrWhiteSpace(obj.Contrasenia))
            {
                obj.Contrasenia = null;
            }

            // 3. Proceder a la actualización
            // Si obj.Contrasenia viene vacío, el SP mantendrá la actual gracias al ISNULL
            return usuarioDALC.Actualizar(obj);
        }

        public UsuarioBE ObtenerPorId(long id)
        {
            return usuarioDALC.ObtenerPorId(id);
        }
        public UsuarioBE BuscarPorUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new Exception("El nombre de usuario es obligatorio para la busqueda.");

            return usuarioDALC.ObtenerPorUsername(username);
        }

    }
}
