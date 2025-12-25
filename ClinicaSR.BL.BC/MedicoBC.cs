using ClinicaSR.BL.BE;
using ClinicaSR.DL.DALC;
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

        public List<MedicoBE> BuscarMedicos(string filtro)
        {
            return medicoDALC.BuscarMedicos(filtro);
        }

        public bool EliminarMedico(int id)
        {
            return medicoDALC.Eliminar(id);
        }

        public MedicoBE ObtenerPorId(long idMedico)
        {
            return medicoDALC.ObtenerPorId(idMedico);
        }

    }
}
