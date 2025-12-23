using ClinicaSR.BL.BE;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ClinicaSR.DL.DALC
{
    public class EspecialidadDALC
    {
        public List<EspecialidadBE> ListarEspecialidades()
        {
            List<EspecialidadBE> lista = new List<EspecialidadBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Especialidades", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        EspecialidadBE esp = new EspecialidadBE
                        {
                            ID_Especialidad = dr.GetInt32(0),
                            Nombre = dr.GetString(1)
                        };
                        lista.Add(esp);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar especialidades: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }

            return lista;
        }

        // 2. Insertar especialidad
        public int Insertar(EspecialidadBE especialidadBE)
        {
            int idInsertado = 0;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Insertar_Especialidad", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", especialidadBE.Nombre);

                SqlParameter salida = new SqlParameter("@ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(salida);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    idInsertado = Convert.ToInt32(salida.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al insertar especialidad: " + ex.Message);
                }
            }

            return idInsertado;
        }

        // 3. Eliminar especialidad
        public bool Eliminar(int id)
        {
            bool eliminado = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Eliminar_Especialidad", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", id);

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
                    Console.WriteLine("Error al eliminar especialidad: " + ex.Message);
                }
            }

            return eliminado;
        }

        // 4. Actualizar especialidad
        public bool Actualizar(EspecialidadBE especialidadBE)
        {
            bool actualizado = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Actualizar_Especialidad", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID", especialidadBE.ID_Especialidad);
                cmd.Parameters.AddWithValue("@Nombre", especialidadBE.Nombre);

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
                    Console.WriteLine("Error al actualizar especialidad: " + ex.Message);
                }
            }

            return actualizado;
        }

    }
}
