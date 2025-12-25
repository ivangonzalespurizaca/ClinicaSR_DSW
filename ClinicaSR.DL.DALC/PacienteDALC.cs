using ClinicaSR.BL.BE;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ClinicaSR.DL.DALC
{
    public class PacienteDALC
    {
        public PacienteBE registrarPaciente(PacienteBE pacienteBE)
        {
            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_InsertarPaciente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombres", pacienteBE.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", pacienteBE.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", pacienteBE.DNI);
                cmd.Parameters.AddWithValue("@Fecha_Nacimiento",
                    (object)pacienteBE.Fecha_Nacimiento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono",
                    (object)pacienteBE.Telefono ?? DBNull.Value);

                try
                {
                    con.Open();

                    // Devuelve el ID generado por SCOPE_IDENTITY()
                    pacienteBE.ID_Paciente = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al registrar paciente: " + ex.Message);
                }
            }

            // 👇 ESTO FALTABA
            return pacienteBE;
        }


       
        // 2. Listar pacientes YA TA
        public List<PacienteBE> ListarPacientes()
        {
            List<PacienteBE> lista = new List<PacienteBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_ListarPacientes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        PacienteBE paciente = new PacienteBE
                        {
                            ID_Paciente = Convert.ToInt64(dr["ID_Paciente"]),
                            Nombres = dr["Nombres"].ToString(),
                            Apellidos = dr["Apellidos"].ToString(),
                            Telefono = dr["Telefono"] == DBNull.Value
                                            ? null
                                            : dr["Telefono"].ToString(),
                            DNI = dr["DNI"].ToString(),
                            Fecha_Nacimiento = dr["Fecha_Nacimiento"] == DBNull.Value
                                            ? (DateTime?)null
                                            : Convert.ToDateTime(dr["Fecha_Nacimiento"]),
                            TieneCitas = Convert.ToBoolean(dr["TieneCitas"])
                        };

                        lista.Add(paciente);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar pacientes: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }

            return lista;
        }


        // 3. Actualizar paciente
        
        public bool ActualizarPaciente(PacienteBE pacienteBE)
        {
            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_ActualizarPaciente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Paciente", pacienteBE.ID_Paciente);
                cmd.Parameters.AddWithValue("@Nombres", pacienteBE.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", pacienteBE.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", pacienteBE.DNI);
                cmd.Parameters.AddWithValue("@Fecha_Nacimiento",
                    (object)pacienteBE.Fecha_Nacimiento ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Telefono",
                    (object)pacienteBE.Telefono ?? DBNull.Value);

                try
                {

                    con.Open();
                    int filas = Convert.ToInt32(cmd.ExecuteScalar());
                    return filas > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al actualizar paciente: " + ex.Message);
                    return false;
                }
            }
        }



        // 4. Eliminar paciente
        public bool eliminarPaciente(int idPaciente)
        {
            bool eliminado = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Eliminar_Paciente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Paciente", idPaciente);

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
                    Console.WriteLine("Error al eliminar paciente: " + ex.Message);
                }
            }

            return eliminado;
        }


        // 5. Buscar paciente por ID
        public PacienteBE ObtenerPacientePorId(long idPaciente)
        {
            PacienteBE paciente = null;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_ObtenerPacientePorId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_Paciente", idPaciente);

                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        paciente = new PacienteBE
                        {
                            ID_Paciente = dr.GetInt64(0),
                            Nombres = dr.GetString(1),
                            Apellidos = dr.GetString(2),
                            DNI = dr.GetString(3),
                            Fecha_Nacimiento = dr.IsDBNull(4) ? null : dr.GetDateTime(4),
                            Telefono = dr.IsDBNull(5) ? null : dr.GetString(5)
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener paciente: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }

            return paciente;
        }



        public List<PacienteBE> BuscarPorCriterio(string criterio)
        {
            List<PacienteBE> lista = new List<PacienteBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Paciente_Buscar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Definición explícita del parámetro
                SqlParameter param = new SqlParameter("@Criterio", SqlDbType.VarChar, 100);
                param.Value = (object)criterio ?? DBNull.Value;
                cmd.Parameters.Add(param);

                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(MapearPaciente(dr));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Loguear el error antes de lanzar la excepción
                    throw new Exception("Error en PacienteDALC.BuscarPorCriterio: " + ex.Message);
                }
            }
            return lista;
        }

        private PacienteBE MapearPaciente(SqlDataReader dr)
        {
            PacienteBE paciente = new PacienteBE();

            // Mapeo siguiendo el estándar de GetOrdinal para mayor seguridad
            paciente.ID_Paciente = dr.GetInt64(dr.GetOrdinal("ID_Paciente"));
            paciente.Nombres = dr["Nombres"].ToString();
            paciente.Apellidos = dr["Apellidos"].ToString();
            paciente.DNI = dr["DNI"].ToString();

            // Manejo de nulos para el teléfono
            paciente.Telefono = dr["Telefono"] == DBNull.Value ? "" : dr["Telefono"].ToString();

            return paciente;
        }





    }
}
