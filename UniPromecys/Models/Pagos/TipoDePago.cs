namespace UniPromecys.Models.Pagos
{
    public class TipoDePago:FormViewModel
    {
        public Int32 IdTipoDePago { get; set; }

        public String Descripcion { get; set; }

        public String CodigoInterno { get; set; }

        public Decimal Precio { get; set; }

        public Boolean Anio { get; set; }

        public String LleaAnio { get; set; }

        public Boolean Mes { get; set; }

        public String LlevaMes { get; set; }

        public Int32 IdUsuario { get; set; }
    }
}
