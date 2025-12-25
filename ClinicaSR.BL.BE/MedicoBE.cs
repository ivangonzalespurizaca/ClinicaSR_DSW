using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BE
{
    public class MedicoBE
    {
        public long ID_Medico { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string DNI { get; set; }
        public string Nro_Colegiatura { get; set; }
        public string Telefono { get; set; }
        public EspecialidadBE EspecialidadBE { get; set; }

        public MedicoBE()
        {
            EspecialidadBE = new EspecialidadBE();
        }

        // Propiedad calculada para mostrar en el modal
        public string NombreCompleto => $"{Nombres} {Apellidos}";
    }
}
