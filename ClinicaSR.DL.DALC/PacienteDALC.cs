using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.DL.DALC
{
    public class PacienteDALC
    {
        public PacienteBE registrarPaciente(PacienteBE pacienteBE)
        {
            using (SqlConnection con = ConexionDALC.GetConnectionBDTiendaCarros())
            {
                SqlCommand cmd = new SqlCommand("USP_Registrar_Paciente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                cmd.Parameters.AddWithValue("@Nombres", pacienteBE.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", pacienteBE.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", pacienteBE.DNI);
                cmd.Parameters.AddWithValue("@Fecha_nacimiento", pacienteBE.Fecha_nacimiento);
                cmd.Parameters.AddWithValue("@Telefono", (object)pacienteBE.Telefono ?? DBNull.Value);

                // Parámetro de salida
                SqlParameter idSalida = new SqlParameter("@ID_Paciente", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(idSalida);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    pacienteBE.ID_Paciente = Convert.ToInt32(idSalida.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al registrar paciente: " + ex.Message);
                }
            }

            return pacienteBE;
        }

        // 2. Listar pacientes
        public List<PacienteBE> ListarPacientes()
        {
            List<PacienteBE> lista = new List<PacienteBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDTiendaCarros())
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Pacientes", con);
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
                            ID_Paciente = dr.GetInt32(0),
                            Nombres = dr.GetString(1),
                            Apellidos = dr.GetString(2),
                            DNI = dr.GetString(3),
                            Fecha_nacimiento = dr.GetDateTime(4),
                            Telefono = dr.IsDBNull(5) ? null : dr.GetString(5)
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
        public bool actualizarPaciente(PacienteBE pacienteBE)
        {
            bool actualizado = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDTiendaCarros())
            {
                SqlCommand cmd = new SqlCommand("USP_Actualizar_Paciente", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Paciente", pacienteBE.ID_Paciente);
                cmd.Parameters.AddWithValue("@Nombres", pacienteBE.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", pacienteBE.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", pacienteBE.DNI);
                cmd.Parameters.AddWithValue("@Fecha_nacimiento", pacienteBE.Fecha_nacimiento);
                cmd.Parameters.AddWithValue("@Telefono", (object)pacienteBE.Telefono ?? DBNull.Value);

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
                    Console.WriteLine("Error al actualizar paciente: " + ex.Message);
                }
            }

            return actualizado;
        }

        // 4. Eliminar paciente
        public bool eliminarPaciente(int idPaciente)
        {
            bool eliminado = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDTiendaCarros())
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
        public PacienteBE buscarPacientePorId(int idPaciente)
        {
            PacienteBE paciente = null;

            using (SqlConnection con = ConexionDALC.GetConnectionBDTiendaCarros())
            {
                SqlCommand cmd = new SqlCommand("USP_Obtener_Paciente_PorID", con);
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
                            ID_Paciente = dr.GetInt32(0),
                            Nombres = dr.GetString(1),
                            Apellidos = dr.GetString(2),
                            DNI = dr.GetString(3),
                            Fecha_nacimiento = dr.GetDateTime(4),
                            Telefono = dr.IsDBNull(5) ? null : dr.GetString(5)
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al buscar paciente por ID: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }

            return paciente;
        }
    }
}
