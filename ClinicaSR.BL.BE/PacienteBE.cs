using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BE
{
    public class PacienteBE
    {
        public long ID_Paciente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string DNI { get; set; }
        public DateTime? Fecha_Nacimiento { get; set; }
        public string? Telefono { get; set; }

    }
}
