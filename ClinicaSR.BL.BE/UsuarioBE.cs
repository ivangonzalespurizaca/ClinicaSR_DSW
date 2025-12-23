using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace ClinicaSR.BL.BE
{
    public class UsuarioBE
    {
        public long ID_Usuario { get; set; }
        public string Username { get; set; }
        public string Contrasenia { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string DNI { get; set; }
        public string? Telefono { get; set; }
        public string? Img_Perfil { get; set; }
        public string? Correo { get; set; }
        public Rol Rol { get; set; }
        public EstadoUsuario Estado { get; set; }
    }

    public enum Rol
    {
        ADMINISTRADOR,
        RECEPCIONISTA,
        CAJERO
    }

    public enum EstadoUsuario
    {
        ACTIVO,
        INACTIVO
    }

}