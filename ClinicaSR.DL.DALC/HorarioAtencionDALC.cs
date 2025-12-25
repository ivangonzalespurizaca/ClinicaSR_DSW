using ClinicaSR.BL.BE;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ClinicaSR.DL.DALC
{
    public class HorarioAtencionDALC
    {
        public bool InsertarHorario(HorarioAtencionBE entidad)
        {
            bool exito = false;
            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Horario_Registrar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Medico", entidad.MedicoBE.ID_Medico);
                cmd.Parameters.AddWithValue("@Dia_Semana", entidad.Dia_Semana.ToString());
                cmd.Parameters.AddWithValue("@Horario_Entrada", entidad.Horario_Entrada);
                cmd.Parameters.AddWithValue("@Horario_Salida", entidad.Horario_Salida);
                try
                {
                    con.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    exito = filasAfectadas > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error técnico al registrar el horario: " + ex.Message);
                }
            }
            return exito;
        }

        public List<HorarioAtencionBE> ListarHorariosPorMedico(long idMedico)
            {
                List<HorarioAtencionBE> lista = new List<HorarioAtencionBE>();

                using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
                {
                    SqlCommand cmd = new SqlCommand("USP_Horarios_ListarPorMedico", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_Medico", idMedico);

                    SqlDataReader dr = null;

                    try
                    {
                        con.Open();
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            HorarioAtencionBE horario = new HorarioAtencionBE
                            {
                                ID_Horario = dr.GetInt64(0),
                                Dia_Semana = (DiaSemana)Enum.Parse(typeof(DiaSemana), dr.GetString(1).Trim().ToUpper(), true),
                                Horario_Entrada = dr.GetTimeSpan(dr.GetOrdinal("Horario_Entrada")),
                                Horario_Salida = dr.GetTimeSpan(dr.GetOrdinal("Horario_Salida")),

                                MedicoBE = new MedicoBE { ID_Medico = idMedico }
                            };
                            lista.Add(horario);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al listar horarios: " + ex.Message);
                    }
                    finally
                    {
                        if (dr != null && !dr.IsClosed) dr.Close();
                    }
                }
                return lista;
            }

        public void EliminarHorario(long idHorario)
        {
            bool exito = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Horario_Eliminar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Horario", idHorario);

                try
                {
                    con.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    exito = filasAfectadas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error técnico al eliminar el horario: " + ex.Message);
                }
            }
        }

        public DisponibilidadMedicaBE VerificarDisponibilidad(long idMedico, DateTime fecha)
        {
            DisponibilidadMedicaBE disponibilidad = new DisponibilidadMedicaBE();

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Horario_VerificarDisponibilidad", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_Medico", idMedico);
                cmd.Parameters.AddWithValue("@Fecha", fecha);

                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();

                    // --- PRIMER RESULTADO: Horarios de atención configurados ---
                    while (dr.Read())
                    {
                        disponibilidad.HorariosConfigurados.Add(new HorarioAtencionBE
                        {
                            ID_Horario = dr.GetInt64(dr.GetOrdinal("ID_Horario")),
                            Horario_Entrada = (TimeSpan)dr["Horario_Entrada"],
                            Horario_Salida = (TimeSpan)dr["Horario_Salida"]
                        });
                    }

                    // --- SEGUNDO RESULTADO: Citas ya ocupadas ---
                    // NextResult() mueve el puntero al siguiente SELECT del SP
                    if (dr.NextResult())
                    {
                        while (dr.Read())
                        {
                            disponibilidad.HorasOcupadas.Add((TimeSpan)dr["Hora_Cita"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en HorarioAtencionDALC.VerificarDisponibilidad: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }
            return disponibilidad;
        }

    }
    }

