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
        public List<UsuarioBE> listarUsuarios()
        {
            SqlConnection con = new SqlConnection();
            con = ConexionDALC.GetConnectionBDSeg();
            List<UsuarioBE> listaUsuarios = new List<UsuarioBE>();
            SqlCommand cmd = new SqlCommand("USP_Listar_Usuarios", con);
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
                        Nombres = dr.GetString(1),
                        Apellidos = dr.GetString(2),
                    };
                    listaUsuarios.Add(usuario);
                }
            }
            catch (Exception sqlex)
            {
                throw new Exception("Error de base de datos al validar usuario: " + sqlex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return listaUsuarios;
        }

        public UsuarioBE Login(string username, string password)
        {
            UsuarioBE usuarioBE = null;
            SqlConnection con = new SqlConnection();
            con = ConexionDALC.GetConnectionBDSeg();
            SqlCommand cmd = new SqlCommand("USP_Usuario_LOGIN", con);
            SqlDataReader dr = null;
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Contrasenia", password);
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    usuarioBE = new UsuarioBE();
                    usuarioBE.ID_Usuario = dr.GetInt64(0);
                    usuarioBE.Username = dr.GetString(1);
                    usuarioBE.Nombres = dr.GetString(2);
                    usuarioBE.Apellidos = dr.GetString(3);
                    usuarioBE.Rol = (Rol)Enum.Parse(typeof(Rol), dr.GetString(4));
                    usuarioBE.Estado = (EstadoUsuario)Enum.Parse(typeof(EstadoUsuario), dr.GetString(5));
                    usuarioBE.Img_Perfil = dr.IsDBNull(6) ? null : dr.GetString(6);
                    usuarioBE.Correo = dr.IsDBNull(7) ? null : dr.GetString(7);
                }
                con.Close();
            }
            catch (SqlException ex)
            {
                throw new Exception("Error en el servidor de base de datos: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el inicio de sesión: " + ex.Message);
            }
            return usuarioBE;
        }
    }
}
