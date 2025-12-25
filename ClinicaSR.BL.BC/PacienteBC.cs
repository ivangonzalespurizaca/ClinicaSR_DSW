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

        public PacienteBE RegistrarPaciente(PacienteBE pacienteBE)
        {
            return pacienteDALC.registrarPaciente(pacienteBE);
        }

        public bool ActualizarPaciente(PacienteBE pacienteBE)
        {
            return pacienteDALC.ActualizarPaciente(pacienteBE);
        }

        public bool EliminarPaciente(int idPaciente)
        {
            return pacienteDALC.eliminarPaciente(idPaciente);
        }

        public PacienteBE BuscarPacientePorId(int idPaciente)
        {
            return pacienteDALC.ObtenerPacientePorId(idPaciente);
        }

        public List<PacienteBE> buscarPacientePorFiltro(string filtro)
        {
            return pacienteDALC.BuscarPorCriterio(filtro);
        }

    }
}
