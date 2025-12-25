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
        // 1. OBTENER POR ID
        public UsuarioBE ObtenerPorId(long idUsuario)
        {
            UsuarioBE usuario = null;
            using (SqlConnection con = ConexionDALC.GetConnectionBDSeg())
            {
                SqlCommand cmd = new SqlCommand("USP_Usuario_ObtenerPorId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) usuario = MapearUsuario(dr);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en UsuarioDALC.ObtenerPorId: " + ex.Message);
                }
            }
            return usuario;
        }

        // 2. OBTENER POR USERNAME
        public UsuarioBE ObtenerPorUsername(string username)
        {
            UsuarioBE usuario = null;
            using (SqlConnection con = ConexionDALC.GetConnectionBDSeg())
            {
                SqlCommand cmd = new SqlCommand("USP_Usuario_ObtenerPorUsername", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) usuario = MapearUsuario(dr);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en UsuarioDALC.ObtenerPorUsername: " + ex.Message);
                }
            }
            return usuario;
        }

        // 3. REGISTRAR USUARIO
        public bool Registrar(UsuarioBE obj)
        {
            bool exito = false;
            using (SqlConnection con = ConexionDALC.GetConnectionBDSeg())
            {
                SqlCommand cmd = new SqlCommand("USP_Usuario_Registrar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", obj.Username);
                cmd.Parameters.AddWithValue("@Contrasenia", obj.Contrasenia);
                cmd.Parameters.AddWithValue("@Nombres", obj.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", obj.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", obj.DNI);
                cmd.Parameters.AddWithValue("@Telefono", obj.Telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Img_Perfil", obj.Img_Perfil ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", obj.Correo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Rol", obj.Rol.ToString());

                try
                {
                    con.Open();
                    int filas = cmd.ExecuteNonQuery();
                    exito = filas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en UsuarioDALC.Registrar: " + ex.Message);
                }
            }
            return exito;
        }

        // 4. ACTUALIZAR USUARIO
        public bool Actualizar(UsuarioBE obj)
        {
            bool exito = false;
            using (SqlConnection con = ConexionDALC.GetConnectionBDSeg())
            {
                SqlCommand cmd = new SqlCommand("USP_Usuario_Actualizar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ID_Usuario", obj.ID_Usuario);
                cmd.Parameters.AddWithValue("@Username", obj.Username);
                // Si la contraseña es nula, el SP usa ISNULL para mantener la actual
                cmd.Parameters.AddWithValue("@Contrasenia", string.IsNullOrEmpty(obj.Contrasenia) ? DBNull.Value : (object)obj.Contrasenia);
                cmd.Parameters.AddWithValue("@Nombres", obj.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", obj.Apellidos);
                cmd.Parameters.AddWithValue("@DNI", obj.DNI);
                cmd.Parameters.AddWithValue("@Telefono", obj.Telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Correo", obj.Correo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Img_Perfil", obj.Img_Perfil ?? (object)DBNull.Value);

                try
                {
                    con.Open();
                    int filas = cmd.ExecuteNonQuery();
                    exito = filas > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en UsuarioDALC.Actualizar: " + ex.Message);
                }
            }
            return exito;
        }

        public UsuarioBE ObtenerPorDni(string dni)
        {
            UsuarioBE usuario = null;
            using (SqlConnection con = ConexionDALC.GetConnectionBDSeg())
            {
                // Consulta directa o puedes crear un SP similar a ObtenerPorUsername
                SqlCommand cmd = new SqlCommand("SELECT * FROM Usuario WHERE DNI = @DNI", con);
                cmd.Parameters.AddWithValue("@DNI", dni);
                try
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) usuario = MapearUsuario(dr);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al validar DNI: " + ex.Message);
                }
            }
            return usuario;
        }

        // MÉTODO AUXILIAR PARA MAPEADO (Reutilizable)
        private UsuarioBE MapearUsuario(SqlDataReader dr)
        {
            return new UsuarioBE
            {
                ID_Usuario = dr.GetInt64(dr.GetOrdinal("ID_Usuario")),
                Username = dr.GetString(dr.GetOrdinal("Username")),
                Contrasenia = dr.GetString(dr.GetOrdinal("Contrasenia")),
                Nombres = dr.GetString(dr.GetOrdinal("Nombres")),
                Apellidos = dr.GetString(dr.GetOrdinal("Apellidos")),
                DNI = dr.GetString(dr.GetOrdinal("DNI")),
                Telefono = dr.IsDBNull(dr.GetOrdinal("Telefono")) ? null : dr.GetString(dr.GetOrdinal("Telefono")),
                Img_Perfil = dr.IsDBNull(dr.GetOrdinal("Img_Perfil")) ? null : dr.GetString(dr.GetOrdinal("Img_Perfil")),
                Correo = dr.IsDBNull(dr.GetOrdinal("Correo")) ? null : dr.GetString(dr.GetOrdinal("Correo")),
                Rol = (Rol)Enum.Parse(typeof(Rol), dr.GetString(dr.GetOrdinal("Rol")), true),
                Estado = (EstadoUsuario)Enum.Parse(typeof(EstadoUsuario), dr.GetString(dr.GetOrdinal("Estado")), true)
            };
        }
    }
}
