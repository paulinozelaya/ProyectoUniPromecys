using UniPromecys.Models.Empleado;

namespace UniPromecys.Models.Cuentas
{
    public class CuentasFormModel:FormViewModel
    {
        public CuentasItemModel CuentasItemModel { get; set; }

        public Int32 IdUsuario { get; set; }

        public Int32 IdModulo { get; set; }

        public Int32 IdPermiso { get; set; }

        public List<Modulos> modulosList { get; set; }

        public List<Permisos> permisosList { get; set; }

        public List<EmpleadoItemModel> empleadoList { get; set; }

        public List<DetalleRolCuentaItem> DetalleCuenta { get; set; }
    }
}
