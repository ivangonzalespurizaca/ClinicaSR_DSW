using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BC
{
    public class HorarioAtencionBC
    {
        private HorarioAtencionDALC horarioDALC = new HorarioAtencionDALC();

        public List<HorarioAtencionBE> ListarHorarios()
        {
            return horarioDALC.ListarHorarios();
        }

        public HorarioAtencionBE RegistrarHorario(HorarioBE horarioBE)
        {
            return horarioDALC.RegistrarHorario(horarioBE);
        }

        public HorarioAtencionBE EditarHorario(HorarioBE horarioBE)
        {
            return horarioDALC.EditarHorario(horarioBE);
        }

        public void EliminarHorario(int idHorario)
        {
            horarioDALC.EliminarHorarioPorId(idHorario);
        }
    }
}
