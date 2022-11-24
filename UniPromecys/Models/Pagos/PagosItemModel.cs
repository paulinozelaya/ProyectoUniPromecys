namespace UniPromecys.Models.Pagos
{
    public class PagosItemModel
    {
        public Int32 IdPago { get; set; }

        public Int32 IdUsuario { get; set; }

        public Int32 IdTipoDePago { get; set; }

        public String Carnet { get; set; }

        public String NombreCompleto { get; set; }

        public Decimal Descuento { get; set; }

        public String Descripcion { get; set; }

        public Decimal Precio { get; set; }
        public Decimal SubTotal { get; set; }

        public Decimal Total   { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }
    }
}
