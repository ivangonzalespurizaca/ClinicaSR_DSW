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
       
            // 1. Listar horarios
            public List<HorarioAtencionBE> ListarHorarios()
            {
                List<HorarioAtencionBE> lista = new List<HorarioAtencionBE>();

                using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
                {
                    SqlCommand cmd = new SqlCommand("USP_Listar_Horarios", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader dr = null;

                    try
                    {
                        con.Open();
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            HorarioAtencionBE horario = new HorarioAtencionBE
                            {
                                ID_Horario = dr.GetInt32(0),
                                MedicoBE = new MedicoBE
                                {
                                    ID_Medico = dr.GetInt32(1),
                                    Nombres = dr.GetString(2),
                                    Apellidos = dr.GetString(3)
                                },
                                Dia_Semana = (DiaSemana)Enum.Parse(typeof(DiaSemana), dr.GetString(4)),
                                Horario_Entrada = dr.GetTimeSpan(5),
                                Horario_Salida = dr.GetTimeSpan(6)
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

            // 2. Registrar horario
            public HorarioAtencionBE registrarHorario(HorarioAtencionBE horarioBE)
            {
                using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
                {
                    SqlCommand cmd = new SqlCommand("USP_Insertar_Horario", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MedicoID", horarioBE.MedicoBE.ID_Medico);
                    cmd.Parameters.AddWithValue("@Dia_Semana", horarioBE.Dia_Semana.ToString());
                    cmd.Parameters.AddWithValue("@Horario_Entrada", horarioBE.Horario_Entrada);
                    cmd.Parameters.AddWithValue("@Horario_Salida", horarioBE.Horario_Salida);

                    SqlParameter idSalida = new SqlParameter("@ID_Horario", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idSalida);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        horarioBE.ID_Horario = Convert.ToInt32(idSalida.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al registrar horario: " + ex.Message);
                        horarioBE = null;
                    }
                }

                return horarioBE;
            }

            // 3. Editar horario
            public HorarioAtencionBE editarHorario(HorarioAtencionBE horarioBE)
            {
                using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
                {
                    SqlCommand cmd = new SqlCommand("USP_Actualizar_Horario", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID_Horario", horarioBE.ID_Horario);
                    cmd.Parameters.AddWithValue("@MedicoID", horarioBE.MedicoBE.ID_Medico);
                    cmd.Parameters.AddWithValue("@Dia_Semana", horarioBE.Dia_Semana.ToString());
                    cmd.Parameters.AddWithValue("@Horario_Entrada", horarioBE.Horario_Entrada);
                    cmd.Parameters.AddWithValue("@Horario_Salida", horarioBE.Horario_Salida);

                    SqlParameter result = new SqlParameter("@Result", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(result);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        if (!Convert.ToBoolean(result.Value))
                            horarioBE = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al editar horario: " + ex.Message);
                        horarioBE = null;
                    }
                }

                return horarioBE;
            }

            // 4. Eliminar horario
            public bool eliminarHorarioPorId(int idHorario)
            {
                bool eliminado = false;

                using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
                {
                    SqlCommand cmd = new SqlCommand("USP_Eliminar_Horario", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ID_Horario", idHorario);

                    SqlParameter result = new SqlParameter("@Result", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(result);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        eliminado = Convert.ToBoolean(result.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al eliminar horario: " + ex.Message);
                    }
                }

                return eliminado;
            }

        public void EliminarHorarioPorId(int idHorario)
        {
            throw new NotImplementedException();
        }

        public HorarioAtencionBE EditarHorario(HorarioAtencionBE horarioBE)
        {
            throw new NotImplementedException();
        }

        public HorarioAtencionBE RegistrarHorario(HorarioAtencionBE horarioBE)
        {
            throw new NotImplementedException();
        }

        

        
    }
    }

