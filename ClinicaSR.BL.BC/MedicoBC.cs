using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BC
{
    public class MedicoBC
    {
        private MedicoDALC medicoDALC = new MedicoDALC();

        public List<MedicoBE> ListarMedicos()
        {
            return medicoDALC.ListarMedicos();
        }

        public int InsertarMedico(MedicoBE medicoBE)
        {
            return medicoDALC.Insertar(medicoBE);
        }

        public bool ActualizarMedico(MedicoBE medicoBE)
        {
            return medicoDALC.Actualizar(medicoBE);
        }

        public bool EliminarMedico(int id)
        {
            return medicoDALC.Eliminar(id);
        }
    }
}
