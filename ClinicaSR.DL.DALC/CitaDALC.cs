using ClinicaSR.BL.BE;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ClinicaSR.DL.DALC
{
    public class CitaDALC
    {
        public List<CitaBE> ListarTodo()
        {
            List<CitaBE> lista = new List<CitaBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Cita_ListarTodo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(MapearCita(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en CitaDALC.ListarTodo: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }
            return lista;
        }

        public List<CitaBE> BuscarPorCriterio(string criterio)
        {
            List<CitaBE> lista = new List<CitaBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Cita_BuscarPorCriterio", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Criterio", criterio ?? (object)DBNull.Value);
                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(MapearCita(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en CitaDALC.BuscarPorCriterio: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }
            return lista;
        }

        public long RegistrarCita(CitaBE obj)
        {
            long idGenerado = 0;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Cita_Registrar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Medico", obj.MedicoBE.ID_Medico);
                cmd.Parameters.AddWithValue("@ID_Paciente", obj.PacienteBE.ID_Paciente);
                cmd.Parameters.AddWithValue("@ID_Usuario", obj.ID_Usuario);
                cmd.Parameters.AddWithValue("@Fecha_Cita", obj.Fecha_Cita); 
                cmd.Parameters.AddWithValue("@Hora_Cita", obj.Hora_Cita);   
                cmd.Parameters.AddWithValue("@Motivo", obj.Motivo ?? (object)DBNull.Value);

                try
                {
                    con.Open();
                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null && resultado != DBNull.Value)
                    {
                        idGenerado = Convert.ToInt64(resultado);
                    }
                }
                catch (SqlException sqlex)
                {
                    throw new Exception(sqlex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en CitaDALC.Registrar: " + ex.Message);
                }
            }
            return idGenerado;
        }

        private CitaBE MapearCita(SqlDataReader dr)
        {
            CitaBE cita = new CitaBE();

            // 1. Datos propios de la Cita
            cita.ID_Cita = dr.GetInt64(dr.GetOrdinal("ID_Cita"));
            cita.Fecha_Cita = dr.GetDateTime(dr.GetOrdinal("Fecha_Cita"));
            cita.Hora_Cita = (TimeSpan)dr["Hora_Cita"];
            cita.Estado = (EstadoCita)Enum.Parse(typeof(EstadoCita), dr["Estado"].ToString(), true);
            cita.Motivo = dr["Motivo"] == DBNull.Value ? "" : dr["Motivo"].ToString();

            // 2. Datos del Paciente (Concatenados desde SQL)
            cita.PacienteBE = new PacienteBE
            {
                // Asignamos el nombre concatenado al campo Nombres para facilitar la vista
                Nombres = dr["PacienteNombre"].ToString(),
                DNI = dr["PacienteDNI"].ToString(),
                Apellidos = "" // Queda vacío porque ya viene incluido en Nombres
            };

            // 3. Datos del Médico (Concatenados desde SQL)
            cita.MedicoBE = new MedicoBE
            {
                Nombres = dr["MedicoNombre"].ToString(),
                Apellidos = ""
            };

            // 4. Especialidad del Médico
            cita.MedicoBE.EspecialidadBE.Nombre = dr["Especialidad"].ToString();

            return cita;
        }

        // 1. Método para obtener una cita específica por su ID
        public CitaBE ObtenerPorId(long idCita)
        {
            CitaBE cita = null;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Cita_ObtenerPorId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_Cita", idCita);

                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            cita = MapearCita(dr);
                            cita.MedicoBE.ID_Medico = dr.GetInt64(dr.GetOrdinal("ID_Medico"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en CitaDALC.ObtenerPorId: " + ex.Message);
                }
            }
            return cita;
        }

        // 2. Método para actualizar los datos de la cita
        public bool ActualizarCita(CitaBE obj)
        {
            bool exito = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Cita_Actualizar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros requeridos por el SP USP_Cita_Actualizar
                cmd.Parameters.AddWithValue("@ID_Cita", obj.ID_Cita);
                cmd.Parameters.AddWithValue("@Fecha_Cita", obj.Fecha_Cita);
                cmd.Parameters.AddWithValue("@Hora_Cita", obj.Hora_Cita);
                cmd.Parameters.AddWithValue("@Motivo", obj.Motivo ?? (object)DBNull.Value);

                try
                {
                    con.Open();
                    // El SP retorna el ID_Cita si tiene éxito
                    object resultado = cmd.ExecuteScalar();
                    if (resultado != null)
                    {
                        exito = true;
                    }
                }
                catch (SqlException sqlex)
                {
                    // Capturamos los RAISERROR del Procedimiento Almacenado
                    throw new Exception(sqlex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en CitaDALC.ActualizarCita: " + ex.Message);
                }
            }
            return exito;
        }
        public bool CancelarCita(long idCita)
        {
            bool exito = false;
            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Cita_Cancelar", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_Cita", idCita);

                try
                {
                    con.Open();
                    // Cambiamos ExecuteNonQuery por ExecuteScalar
                    object resultado = cmd.ExecuteScalar();

                    // Si el SP devuelve el ID, la operación fue exitosa
                    exito = (resultado != null && resultado != DBNull.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en CitaDALC.CancelarCita: " + ex.Message);
                }
            }
            return exito;
        }

    }
}

