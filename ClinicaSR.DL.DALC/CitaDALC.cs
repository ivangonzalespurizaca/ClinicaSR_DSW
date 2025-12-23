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
        
            // 1. Listar todas las citas
            public List<CitaBE> ListarCitas()
            {
                List<CitaBE> lista = new List<CitaBE>();

                using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
                {
                    SqlCommand cmd = new SqlCommand("USP_Listar_Citas", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader dr = null;

                    try
                    {
                        con.Open();
                        dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            CitaBE cita = new CitaBE
                            {
                                ID_Cita = dr.GetInt32(0),
                                Fecha_Cita = dr.GetDateTime(1),
                                Motivo = dr.GetString(2),
                                Estado = (EstadoCita)Enum.Parse(typeof(EstadoCita), dr.GetString(3)),
                                MedicoBE = new MedicoBE
                                {
                                    ID_Medico = dr.GetInt32(4),
                                    Nombres = dr.GetString(5),
                                    Apellidos = dr.GetString(6)
                                },
                                PacienteBE = new PacienteBE
                                {
                                    ID_Paciente = dr.GetInt32(7),
                                    Nombres = dr.GetString(8),
                                    Apellidos = dr.GetString(9)
                                }
                            };
                            lista.Add(cita);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al listar citas: " + ex.Message);
                    }
                    finally
                    {
                        if (dr != null && !dr.IsClosed) dr.Close();
                    }
                }

                return lista;
            }

            // 2. Actualizar estado de la cita
            public bool Actualizar(int idCita)
            {
                bool actualizado = false;

                using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
                {
                    SqlCommand cmd = new SqlCommand("USP_Actualizar_Cita", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetro de entrada
                    cmd.Parameters.AddWithValue("@ID_Cita", idCita);

                    // Parámetro de salida
                    SqlParameter result = new SqlParameter("@Result", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(result);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        actualizado = Convert.ToBoolean(result.Value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al actualizar cita: " + ex.Message);
                    }
                }

                return actualizado;
            }



        }
    }

