using ClinicaSR.BL.BC;
using ClinicaSR.BL.BE;
using ClinicaSR.DL.DALC;
using Microsoft.Data.SqlClient;

namespace Clinica_SantaRosa.PL.WebApp.Models
{
    public class MedicoModel

    {
        private MedicoBC medicobc = new MedicoBC();

        public List<MedicoBE>listado()
        {
            return medicobc.ListarMedicos();
        }

        public int insertar(MedicoBE medico)
        {
            return medicobc.InsertarMedico(medico);
        }

        public bool actualizar(MedicoBE medico)
        {
            return medicobc.ActualizarMedico(medico);
        }

        public MedicoBE obtenerxid(int id)
        {
               return medicobc.ObtenerPorId(id);
        }

        public bool eliminar(int id)
        {
            return medicobc.EliminarMedico(id);
        }

        public bool ExisteDNI(string dni)
        {
            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM Medico WHERE DNI = @DNI", con);
                cmd.Parameters.AddWithValue("@DNI", dni);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public bool ExisteNroColegiatura(string nroColegiatura)
        {
            using (SqlConnection con = ConexionDALC.GetConnectionBDHospital())
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM Medico WHERE Nro_Colegiatura = @NroColegiatura", con);
                cmd.Parameters.AddWithValue("@NroColegiatura", nroColegiatura);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }




    }
}
