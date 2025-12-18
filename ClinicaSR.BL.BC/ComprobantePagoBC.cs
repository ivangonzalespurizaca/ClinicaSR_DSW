using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaSR.BL.BC
{
    public class ComprobantePagoBC
    {
        private ComprobantePagoDALC comprobanteDALC = new ComprobantePagoDALC();

        public List<ComprobantePagoBE> ListarComprobantes()
        {
            return comprobanteDALC.ListarComprobantes();
        }

        public bool ActualizarComprobante(int id)
        {
            return comprobanteDALC.Actualizar(id);
        }
    }
}
