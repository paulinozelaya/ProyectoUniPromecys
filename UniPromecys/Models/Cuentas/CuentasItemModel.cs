namespace UniPromecys.Models.Cuentas
{
    public class CuentasItemModel:FormViewModel
    {
        public Int32 IdCuenta { get; set; }

        public String Usuario { get; set; }

        public String Contrasenia { get; set; }

        public String NombreCompleto { get; set; }

        public Int32 IdUsuario { get; set; }

        public Int32 IdPersona { get; set; }

        public Int32 IdRol { get; set; }

        public String CodigoInterno { get; set; }
    }
}
