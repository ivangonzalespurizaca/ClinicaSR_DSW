using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BC
{
    public class EspecialidadBC
    {
        private EspecialidadDALC especialidadDALC = new EspecialidadDALC();

        public List<EspecialidadBE> ListarEspecialidades()
        {
            return especialidadDALC.ListarEspecialidades();
        }

        public int InsertarEspecialidad(EspecialidadBE especialidadBE)
        {
            return especialidadDALC.Insertar(especialidadBE);
        }

        public bool ActualizarEspecialidad(EspecialidadBE especialidadBE)
        {
            return especialidadDALC.Actualizar(especialidadBE);
        }

        public bool EliminarEspecialidad(int id)
        {
            return especialidadDALC.Eliminar(id);
        }
    }
}
