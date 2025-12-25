using ClinicaSR.BL.BE;

namespace Clinica_SantaRosa.PL.WebApp.Models
{
    public class GestionarHorariosViewModel
    {
        public MedicoBE Medico { get; set; }
        public List<HorarioAtencionBE> ListaHorarios { get; set; }
        public HorarioAtencionBE NuevoHorario { get; set; }
        public GestionarHorariosViewModel()
        {
            Medico = new MedicoBE();
            ListaHorarios = new List<HorarioAtencionBE>();
            NuevoHorario = new HorarioAtencionBE();
        }
    }
}
