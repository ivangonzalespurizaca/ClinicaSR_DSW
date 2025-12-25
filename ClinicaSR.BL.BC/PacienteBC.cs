using ClinicaSR.BL.BE;
using ClinicaSR.DL.DALC;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BC
{
    public class PacienteBC
    {
        private PacienteDALC pacienteDALC = new PacienteDALC();

        public List<PacienteBE> ListarPacientes()
        {
            return pacienteDALC.ListarPacientes();
        }

        public List<PacienteBE>  buscarPacientePorFiltro(string filtro)
        {
            return pacienteDALC.BuscarPorCriterio(filtro);
        }
    }
}
