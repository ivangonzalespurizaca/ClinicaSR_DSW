using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BC
{
    public class CitaBC
    {
        private CitaDALC citaDALC = new CitaDALC();

        public List<CitaBE> ListarCitas()
        {
            return citaDALC.ListarCitas();
        }

        public bool ActualizarCita(int id)
        {
            return citaDALC.Actualizar(id);
        }
    }
}
