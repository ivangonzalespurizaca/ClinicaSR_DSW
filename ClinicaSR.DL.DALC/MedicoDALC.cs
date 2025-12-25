using ClinicaSR.BL.BE;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ClinicaSR.DL.DALC
{
    public class MedicoDALC
    {
        public List<MedicoBE> ListarMedicos()
        {
            List<MedicoBE> lista = new List<MedicoBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Medicos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        MedicoBE medico = new MedicoBE
                        {
                            ID_Medico = dr.GetInt32(0),
                            Nombres = dr.GetString(1),
                            Apellidos = dr.GetString(2),
                            DNI = dr.GetString(3),
                            Nro_Colegiatura = dr.GetString(4),
                            Telefono = dr.IsDBNull(5) ? null : dr.GetString(5),
                            EspecialidadBE = new EspecialidadBE
                            {
                                ID_Especialidad = dr.GetInt32(6),
                                Nombre = dr.GetString(7)
                            }
                        };
                        lista.Add(medico);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar medicos: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }

            return lista;
        }

        // 2. Insertar médico
        public int Insertar(MedicoBE medicoBE)
        {
            int idInsertado = 0;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Insertar_Medico", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombres", medicoBE.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", medicoBE.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", medicoBE.DNI);
                cmd.Parameters.AddWithValue("@Nro_Colegiatura", medicoBE.Nro_Colegiatura);
                cmd.Parameters.AddWithValue("@Telefono", (object)medicoBE.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EspecialidadID", medicoBE.EspecialidadBE.ID_Especialidad);

                SqlParameter idSalida = new SqlParameter("@ID_Medico", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(idSalida);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    idInsertado = Convert.ToInt32(idSalida.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al insertar medico: " + ex.Message);
                }
            }

            return idInsertado;
        }

        // 3. Actualizar médico
        public bool Actualizar(MedicoBE medicoBE)
        {
            bool actualizado = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Actualizar_Medico", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Medico", medicoBE.ID_Medico);
                cmd.Parameters.AddWithValue("@Nombres", medicoBE.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", medicoBE.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", medicoBE.DNI);
                cmd.Parameters.AddWithValue("@Nro_Colegiatura", medicoBE.Nro_Colegiatura);
                cmd.Parameters.AddWithValue("@Telefono", (object)medicoBE.Telefono ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EspecialidadID", medicoBE.EspecialidadBE.ID_Especialidad);

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
                    Console.WriteLine("Error al actualizar medico: " + ex.Message);
                }
            }

            return actualizado;
        }

        // 4. Eliminar médico
        public bool Eliminar(int id)
        {
            bool eliminado = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Eliminar_Medico", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Medico", id);

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
                    Console.WriteLine("Error al eliminar medico: " + ex.Message);
                }
            }

            return eliminado;
        }

        public List<MedicoBE> BuscarMedicos(string filtro)
        {
            List<MedicoBE> lista = new List<MedicoBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Medico_Buscar", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro ?? "");
                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        MedicoBE medico = new MedicoBE
                        {
                            ID_Medico = dr.GetInt64(0),
                            Nombres = dr.GetString(1),
                            Apellidos = dr.GetString(2),
                            DNI = dr.GetString(3),

                            EspecialidadBE = new EspecialidadBE
                            {
                                Nombre = dr.GetString(4)
                            }
                        };
                        lista.Add(medico);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al buscar medicos: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }
            return lista;
        }
        // 5. Obtener médico por ID
        public MedicoBE ObtenerPorId(long idMedico)
        {
            MedicoBE medico = null;

            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("USP_Medico_ObtenerPorId", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Medico", idMedico);
                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        medico = new MedicoBE
                        {
                            ID_Medico = dr.GetInt64(0),
                            Nombres = dr.GetString(1),
                            Apellidos = dr.GetString(2),
                            DNI = dr.GetString(3),
                            Nro_Colegiatura = dr.GetString(4),
                            Telefono = dr.IsDBNull(5) ? null : dr.GetString(5),

                            EspecialidadBE = new EspecialidadBE
                            {
                                ID_Especialidad = dr.GetInt64(6),
                                Nombre = dr.GetString(7)
                            }
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener medico por ID: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }
            return medico;
        }

    }
}
