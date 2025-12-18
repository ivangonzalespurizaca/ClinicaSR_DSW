using ClinicaSR.BL.BE;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ClinicaSR.DL.DALC
{
    public class UsuarioDALC
    {
        public List<UsuarioBE> listaUsuarios()
        {

            SqlConnection con = new SqlConnection();
            con = ConexionDALC.GetConnectionBDSeg();
            List<UsuarioBE> listaUsuarios = new List<UsuarioBE>();
            // Inicializamos la conexi  ón SQL
            //SqlConnection con = new SqlConnection(cadena);
            SqlCommand cmd = new SqlCommand("ObtenerUsuariosResumen", con);
            SqlDataReader dr = null;

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open(); 
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {


   
                    UsuarioBE usuario = new UsuarioBE
                    {
                        Username = dr.GetString(0),
                        Nombres = dr.GetString (1),
                        Apellidos  = dr.GetString(2),   
                    };


                    listaUsuarios.Add(usuario);
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine("Error al obtener los usuarios: " + ex.Message);
            }
            finally
            {
                
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
            }


            return listaUsuarios;
        }

        public UsuarioBE Login(string username, string password)
        {
            UsuarioBE usuario = null;

            using (SqlConnection con = ConexionDALC.GetConnectionBDSeg())
            {
                SqlCommand cmd = new SqlCommand("USP_Usuario_Login", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                SqlDataReader dr = null;
                try
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        usuario = new UsuarioBE
                        {
                            Id_Usuario = dr.GetInt32(0),
                            Username = dr.GetString(1),
                            Nombres = dr.GetString(2),
                            Apellidos = dr.GetString(3),
                            DNI = dr.IsDBNull(4) ? null : dr.GetString(4),
                            Telefono = dr.IsDBNull(5) ? null : dr.GetString(5),
                            Correo = dr.IsDBNull(6) ? null : dr.GetString(6),
                            rol = (Rol)Enum.Parse(typeof(Rol), dr.GetString(7), true)
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al hacer login: " + ex.Message);
                }
                finally
                {
                    if (dr != null && !dr.IsClosed)
                        dr.Close();
                }
            }

            return usuario;
        }
    }

}
