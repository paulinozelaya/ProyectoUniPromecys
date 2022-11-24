namespace UniPromecys.Models.Pagos
{
    public class PagosFormModel:FormViewModel
    {
        public PagosItemModel Pagos { get; set; }

        public TipoDePago tipoDePago { get; set; }
        
        public List<TipoDePago> TipoDePagoList { get; set; }

        public List<String> Meses { get; set; }
        public List<DetallePagoItemModel> Detalle { get; set; }
    }
}
