namespace UniPromecys.Models.Direccion
{
    public class Municipio
    {
        public Int32 IdMunicipio { get; set; }
        public Int32 IdDepartamento { get; set; }

        public String CodigoInterno { get; set; }

        public String Nombre { get; set; }

        public String Descripcion { get; set; }
    }
}
