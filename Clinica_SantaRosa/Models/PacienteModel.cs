using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;

namespace Clinica_SantaRosa.PL.WebApp.Models
{
    public class PacienteModel
    {

        private PacienteBC pacienteBC = new PacienteBC();

        public List<PacienteBE> lista()
        {
            return pacienteBC.ListarPacientes();
        }

        public PacienteBE RegistrarPaciente(PacienteBE pacienteBE)
        {
            return pacienteBC.RegistrarPaciente(pacienteBE);
        }

        public PacienteBE ObtenerPorId(int idPaciente)
        {
            return pacienteBC.BuscarPacientePorId(idPaciente);
        }

        // Actualizar
        public bool Actualizar(PacienteBE pacienteBE)
        {
            return pacienteBC.ActualizarPaciente(pacienteBE);
        }

        public bool Eliminar(int idPaciente)
        {
            return pacienteBC.EliminarPaciente(idPaciente);
        }

    }
}
