namespace UniPromecys.Models
{
    public class Permisos
    {

        public Int32 IdPermiso { get; set; }

        public Int32 IdRol { get; set; }
        public Int32 IdModulo { get; set; }

        public String CodigoInterno { get; set; }

        public String Nombre { get; set; }

        public String Descripcion { get; set; }
    }
}
