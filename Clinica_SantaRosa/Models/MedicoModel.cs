using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;

namespace Clinica_SantaRosa.PL.WebApp.Models
{
    public class MedicoModel

    {
        private MedicoBC medicobc = new MedicoBC();

        public List<MedicoBE>listado()
        {
            return medicobc.ListarMedicos();
        }

        public int insertar(MedicoBE medico)
        {
            return medicobc.InsertarMedico(medico);
        }

        public bool actualizar(MedicoBE medico)
        {
            return medicobc.ActualizarMedico(medico);
        }

        public MedicoBE obtenerxid(int id)
        {
               return medicobc.ObtenerPorId(id);
        }

        public bool eliminar(int id)
        {
            return medicobc.EliminarMedico(id);
        }


    }
}
