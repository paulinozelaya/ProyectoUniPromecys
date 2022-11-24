using System;

namespace UniPromecys.Models
{
    public class Resultado
    {
        public Boolean Exito { get; set; }
        public String? Descripcion { get; set; }

        public Int32 IdUsuario { get; set; }
        public String? NombreUsuario { get; set; }
        public String? Cargo { get; set; }

        public Int32 IdPago { get; set; }

        public String? Permisos { get; set; }
    }
}
