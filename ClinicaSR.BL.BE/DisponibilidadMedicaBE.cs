using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaSR.BL.BE
{
    public class DisponibilidadMedicaBE
    {
        public List<HorarioAtencionBE> HorariosConfigurados { get; set; } = new List<HorarioAtencionBE>();
        public List<TimeSpan> HorasOcupadas { get; set; } = new List<TimeSpan>();
    }
}
