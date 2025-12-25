using ClinicaSR.BL.BE;
using ClinicaSR.DL.DALC;
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
            return citaDALC.ListarTodo();
        }

        public List<CitaBE> ListarCitasPorCriterio(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
            {
                criterio = null;
            }
            return citaDALC.BuscarPorCriterio(criterio);
        }

        public long RegistrarCita(CitaBE obj)
        {
            try
            {
                // 1. VALIDACIONES DE INTEGRIDAD DE OBJETOS
                if (obj == null)
                    throw new Exception("El objeto Cita no puede ser nulo.");

                if (obj.MedicoBE == null || obj.MedicoBE.ID_Medico <= 0)
                    throw new Exception("Debe asignar un médico válido a la cita.");

                if (obj.PacienteBE == null || obj.PacienteBE.ID_Paciente <= 0)
                    throw new Exception("Debe asignar un paciente válido a la cita.");

                DateTime fechaHoraCita = obj.Fecha_Cita.Date.Add(obj.Hora_Cita);


                // 2. VALIDACIONES DE REGLAS DE NEGOCIO
                // No permitir citas en fechas pasadas
                if (fechaHoraCita < DateTime.Now)
                {
                    throw new Exception("No se puede registrar una cita para una fecha anterior a hoy.");
                }

                // Validar que el motivo no sea una cadena vacía
                if (string.IsNullOrWhiteSpace(obj.Motivo))
                {
                    throw new Exception("El motivo de la cita es obligatorio.");
                }

                // 3. EJECUCIÓN EN CAPA DE DATOS (DALC)
                // Retorna el ID generado por el procedimiento almacenado
                return citaDALC.RegistrarCita(obj);
            }
            catch (Exception ex)
            {
                // Propagamos el error para que la UI lo muestre (incluyendo los RAISERROR de SQL)
                throw new Exception(ex.Message);
            }
        }

        public CitaBE ObtenerCitaPorId(long id)
        {
            if (id <= 0)
                throw new Exception("El ID de la cita no es válido.");

            return citaDALC.ObtenerPorId(id);
        }

        public bool ActualizarCita(CitaBE obj)
        {
            try
            {
                // 1. VALIDACIONES DE INTEGRIDAD
                if (obj == null || obj.ID_Cita <= 0)
                    throw new Exception("No se ha proporcionado un ID de cita válido para actualizar.");

                // 2. VALIDACIONES DE REGLAS DE NEGOCIO (Idénticas al registro)

                // Combinamos fecha y hora para validar que no sea un tiempo pasado
                DateTime fechaHoraCita = obj.Fecha_Cita.Date.Add(obj.Hora_Cita);

                if (fechaHoraCita < DateTime.Now)
                {
                    throw new Exception("No se puede reprogramar una cita para una fecha u hora que ya ha pasado.");
                }

                if (string.IsNullOrWhiteSpace(obj.Motivo))
                {
                    throw new Exception("El motivo de la cita es obligatorio.");
                }

                // 3. EJECUCIÓN EN CAPA DE DATOS
                // El procedimiento almacenado se encargará de validar la disponibilidad del médico
                return citaDALC.ActualizarCita(obj);
            }
            catch (Exception ex)
            {
                // Propagamos el mensaje, incluyendo los RAISERROR de SQL como "El médico ya tiene otra cita..."
                throw new Exception(ex.Message);
            }
        }
        public bool CancelarCita(long id)
        {
            if (id <= 0) throw new Exception("ID de cita no válido.");

            // Aquí podrías agregar validaciones extra, por ejemplo:
            // No permitir cancelar si falta menos de 1 hora para la cita.

            return citaDALC.CancelarCita(id);
        }
    }
}
