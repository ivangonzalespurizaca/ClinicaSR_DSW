using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BE
{
    public class HorarioAtencionBE
    {
        public long ID_Horario { get; set; }
        public MedicoBE MedicoBE { get; set; }
        public DiaSemana Dia_Semana { get; set; }
        public TimeSpan Horario_Entrada { get; set; }
        public TimeSpan Horario_Salida { get; set; }
    }

    public enum DiaSemana
    {
        LUNES,
        MARTES,
        MIERCOLES,
        JUEVES,
        VIERNES,
        SABADO,
        DOMINGO
    }
}
