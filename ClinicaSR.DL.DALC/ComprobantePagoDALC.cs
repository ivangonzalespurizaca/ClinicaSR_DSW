using System;
using System.Collections.Generic;
using System.Text;
using ClinicaSR.BL.BE;

namespace ClinicaSR.DL.DALC
{
    public class ComprobantePagoDALC
    {
        public List<ComprobantePagoBE> ListarComprobantes()
        {
            List<ComprobantePagoBE> lista = new List<ComprobantePagoBE>();

            using (SqlConnection con = ConexionDALC.GetConnectionBDTiendaCarros())
            {
                SqlCommand cmd = new SqlCommand("USP_Listar_Comprobantes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = null;

                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ComprobantePagoBE comp = new ComprobantePagoBE
                        {
                            ID_Comprobante = dr.GetInt32(0),
                            Nombre_Pagador = dr.GetString(1),
                            Apellidos_Pagador = dr.GetString(2),
                            DNI_Pagador = dr.GetString(3),
                            Contacto_Pagador = dr.GetString(4),
                            Fecha_Emision = dr.GetDateTime(5),
                            Monto = dr.GetDecimal(6),
                            Metodo_Pago = (Metodos)Enum.Parse(typeof(Metodos), dr.GetString(7)),
                            Estado_Pago = (Estados)Enum.Parse(typeof(Estados), dr.GetString(8))
                        };
                        lista.Add(comp);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al listar comprobantes: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed) dr.Close();
                }
            }

            return lista;
        }

        // 2. Actualizar estado de comprobante
        public bool Actualizar(int id)
        {
            bool actualizado = false;

            using (SqlConnection con = ConexionDALC.GetConnectionBDTiendaCarros())
            {
                SqlCommand cmd = new SqlCommand("USP_Actualizar_Comprobante", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Comprobante", id);

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
                    Console.WriteLine("Error al actualizar comprobante: " + ex.Message);
                }
            }

            return actualizado;
        }


    }
}
