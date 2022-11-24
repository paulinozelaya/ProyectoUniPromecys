namespace UniPromecys.Models
{
    public class CargoModel:FormViewModel
    {
        public Int32 IdCargo { get; set; }

        public String CodigoInterno { get; set; }

        public String Nombre { get; set; }

        public String Descripcion { get; set; }

        public Int32 IdUsuario { get; set; }
    }
}
