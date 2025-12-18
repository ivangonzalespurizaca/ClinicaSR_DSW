using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BE
{
    public class CitaBE
    {
        public int ID_Cita { get; set; }

        public DateTime Fecha_Cita { get; set; }
        public string Motivo { get; set; }
        public Estado estado { get; set; }

        public MedicoBE MedicoBE { get; set; }  
        public PacienteBE PacienteBE { get; set; }

	    public UsuarioBE UsuarioBE { get; set; }

    }


    public enum Estado
    {
        Pendiente,
        Pagado
    }

        
}
