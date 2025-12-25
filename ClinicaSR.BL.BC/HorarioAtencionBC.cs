using ClinicaSR.BL.BE;
using ClinicaSR.DL.DALC;
using System;
using System.Collections.Generic;
using System.Linq; // Necesario para usar .Any()

namespace ClinicaSR.BL.BC
{
    public class HorarioAtencionBC
    {
        private HorarioAtencionDALC horarioDALC = new HorarioAtencionDALC();

        public List<HorarioAtencionBE> ListarHorarios(long idMedico)
        {
            if (idMedico <= 0) return new List<HorarioAtencionBE>();
            return horarioDALC.ListarHorariosPorMedico(idMedico);
        }

        public bool RegistrarHorario(HorarioAtencionBE horario, out string mensaje)
        {
            mensaje = "";

            // 1. Validaciones básicas
            if (horario == null) throw new ArgumentException("El objeto horario no puede ser nulo.");

            if (horario.MedicoBE == null || horario.MedicoBE.ID_Medico <= 0)
            {
                mensaje = "Debe seleccionar un médico válido.";
                return false;
            }

            if (horario.Horario_Entrada >= horario.Horario_Salida)
            {
                mensaje = "La hora de salida debe ser posterior a la de entrada.";
                return false;
            }

            try
            {
                // 2. Buscar horarios existentes del médico para validar cruces
                var horariosExistentes = horarioDALC.ListarHorariosPorMedico(horario.MedicoBE.ID_Medico);

                // 3. Validar solapamiento (Lógica idéntica a tu código en Spring)
                // 
                bool solapado = horariosExistentes.Any(h =>
                    h.Dia_Semana == horario.Dia_Semana &&
                    horario.Horario_Entrada < h.Horario_Salida &&
                    horario.Horario_Salida > h.Horario_Entrada
                );

                if (solapado)
                {
                    mensaje = "El horario se solapa con otro existente del mismo día para este médico.";
                    return false;
                }

                // 4. Intentar insertar
                return horarioDALC.InsertarHorario(horario);
            }
            catch (Exception ex)
            {
                // Captura errores técnicos o el RAISERROR del procedimiento almacenado
                mensaje = ex.Message;
                return false;
            }
        }

        public bool EliminarHorario(long idHorario, out string mensaje)
        {
            mensaje = "";
            try
            {
                if (idHorario <= 0)
                {
                    mensaje = "ID de horario no válido.";
                    return false;
                }

                horarioDALC.EliminarHorario(idHorario);
                mensaje = "Horario eliminado correctamente.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = "Error al eliminar: " + ex.Message;
                return false;
            }
        }

        public DisponibilidadMedicaBE VerificarDisponibilidad(long idMedico, DateTime fecha)
        {
            try
            {
                if (idMedico <= 0)
                {
                    throw new Exception("Debe seleccionar un médico válido.");
                }

                // 2. LLAMADA A LA CAPA DE DATOS (DALC)
                return horarioDALC.VerificarDisponibilidad(idMedico, fecha);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en HorarioAtencionBC: " + ex.Message);
            }
        }
    }
}