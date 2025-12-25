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

        public HorarioAtencionBE()
        {
            MedicoBE = new MedicoBE();
        }
    }


    public enum DiaSemana
    {
        DOMINGO = 0,
        LUNES = 1,
        MARTES = 2,
        MIERCOLES = 3,
        JUEVES = 4,
        VIERNES = 5,
        SABADO = 6,
        
    }
}
