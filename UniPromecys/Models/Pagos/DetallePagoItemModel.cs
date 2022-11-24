namespace UniPromecys.Models.Pagos
{
    public class DetallePagoItemModel
    {
        public string Descripcion { get; set; }

        public decimal Precio { get; set; }

        public decimal Descuento { get; set; }

        public decimal Total { get; set; }

        public string Mes { get; set; }

        public int Anio { get; set; }

        public string CodigoTipoPago { get; set; }

    }
}
