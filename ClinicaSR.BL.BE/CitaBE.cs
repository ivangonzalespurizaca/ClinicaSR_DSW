using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BE
{
    public class CitaBE
    {
        public long ID_Cita { get; set; }
        public MedicoBE MedicoBE { get; set; }
        public PacienteBE PacienteBE { get; set; }
        public long ID_Usuario { get; set; }
        public DateTime Fecha_Cita { get; set; }
        public TimeSpan Hora_Cita { get; set; }
        public string Motivo { get; set; }
        public EstadoCita Estado { get; set; }
    }

    public enum EstadoCita
    {
        PENDIENTE,
        CONFIRMADO,
        CANCELADO,
        VENCIDO
    }
}
