using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using UniPromecys.Models.Direccion;
using UniPromecys.Models.Pagos;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using Newtonsoft.Json;

using UniPromecys.Models;
using static UniPromecys.Models.Enum;
using UniPromecys.Models.Estudiante;
using static UniPromecys.Models.AccionesControlador;
using UniPromecys.Models.Asignatura;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Reporting.NETCore;
using UniPromecys.Models.Empleado;
using UniPromecys.Date;

namespace UniPromecys.Controllers
{
    public class PagosController : BaseController
    {
        ObtenerDatos obtenerDatos = new ObtenerDatos();
        public string Conexion => obtenerDatos.Conexion();
        public Int32? IdUsuario => Convert.ToInt16(HttpContext.Request.Cookies["IdUsuario"]);

        public String? IdPago;

        public String? Carnet;

        public IActionResult RegistrarTipoPago()
        {
            var modelo = new PagosFormModel
            {
                tipoDePago = new TipoDePago()
            };

            modelo.Accion = Acciones.Nuevo.ToString();
            return View("RegistrarTipoPago", modelo);
        }

        public IActionResult GuardarPago(PagosFormModel pagosFormModel)
        {
            try
            {
                var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
                String connectionString = Conexion;
                String JsonResultado = "";
                SqlConnection cnn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand();
                //DataTable dataTable = new DataTable();
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA;

                if (pagosFormModel.tipoDePago.IdTipoDePago > 0)
                {
                    cmd.CommandText = "EXEC Administracion.prEditarTipoDePago @IdTipoDePago,@CodigoInterno,@Descripcion,@Precio,@LlevaMes,@LlevaAnio,@IdUsuario";
                    cmd.Parameters.AddWithValue("@IdTipoDePago", pagosFormModel.tipoDePago.IdTipoDePago);
                }
                else
                {
                    cmd.CommandText = "EXEC Administracion.prAñadirTipoDePago @CodigoInterno,@Descripcion,@Precio,@LlevaMes,@LlevaAnio,@IdUsuario";
                }
                cmd.Parameters.AddWithValue("@CodigoInterno", pagosFormModel.tipoDePago.CodigoInterno);
                cmd.Parameters.AddWithValue("@Descripcion", pagosFormModel.tipoDePago.Descripcion);
                cmd.Parameters.AddWithValue("@Precio", pagosFormModel.tipoDePago.Precio);
                cmd.Parameters.AddWithValue("@LlevaMes", pagosFormModel.tipoDePago.Mes);
                cmd.Parameters.AddWithValue("@LlevaAnio", pagosFormModel.tipoDePago.Anio);
                cmd.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cnn;
                sqlDA = new SqlDataAdapter(cmd);
                sqlDA.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    JsonResultado = ds.Tables[0].AsEnumerable().Select(h => h.Field<String>("Resultado")).FirstOrDefault();
                }

                Resultado resultado = JsonConvert.DeserializeObject<Resultado>(JsonResultado);
                Boolean Exito = resultado.Exito;
                String Descripcion = resultado.Descripcion;

                Alert(Descripcion, (Exito ? NotificationType.success : NotificationType.warning));

                cnn.Close();

                if (Exito)
                {
                    return pagosFormModel.PermisoAdministrar ? RedirectToAction("TipoPago") : RedirectToAction("Index", "Home");

                }
                else
                {
                    if(pagosFormModel.tipoDePago.IdTipoDePago > 0)
                    {
                        pagosFormModel.Accion = Acciones.Editar.ToString();
                    }
                    return View("RegistrarTipoPago", pagosFormModel);
                }
            }
            catch(Exception e)
            {
                String Descripcion = "Ha ocurrido un error, Contactar al administrador";
                ViewBag.Resultado = Descripcion;
                Alert(Descripcion, NotificationType.error);
                return View("RegistrarTipoPago", pagosFormModel);
            }          
        }

        public IActionResult Editar(Int32 IdTipoDePago)
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                var modelo = new PagosFormModel();
                var Encontrado = ObtenerTipoDePago().FirstOrDefault(x => x.IdTipoDePago == IdTipoDePago);
                modelo.tipoDePago = Encontrado;
                modelo.tipoDePago.Precio = Convert.ToDecimal(modelo.tipoDePago.Precio, new CultureInfo("en-US"));
                modelo.Accion = Acciones.Editar.ToString();
                return View("RegistrarTipoPago", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
            return View();
        }

        public IActionResult Eliminar(Int32 IdTipoDePago)
        {
            try
            {
                var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
                String connectionString = Conexion;
                String JsonResultado = "";
                SqlConnection cnn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand();
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA;

                cmd.CommandText = "EXEC Administracion.prEliminarTipoDePago @IdTipoDePago,@IdUsuario";
                cmd.Parameters.AddWithValue("@IdTipoDePago", IdTipoDePago);
                cmd.Parameters.AddWithValue("@IdUsuario", Convert.ToInt32(IdUsuario));
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cnn;
                sqlDA = new SqlDataAdapter(cmd);
                sqlDA.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    JsonResultado = ds.Tables[0].AsEnumerable().Select(h => h.Field<String>("Resultado")).FirstOrDefault();
                }

                Resultado resultado = JsonConvert.DeserializeObject<Resultado>(JsonResultado);
                Boolean Exito = resultado.Exito;
                String Descripcion = resultado.Descripcion;

                Alert(Descripcion, (Exito ? NotificationType.success : NotificationType.warning));

                cnn.Close();
            }
            catch (Exception e)
            {
                String Descripcion = "Ha ocurrido un error, Contactar al administrador";
                ViewBag.Resultado = Descripcion;
                Alert(Descripcion, NotificationType.error);
            }

            var modelo = new PagosFormModel
            {
                tipoDePago = new TipoDePago()
            };

            modelo.TipoDePagoList = ObtenerTipoDePago();

            return RedirectToAction("TipoPago");
        }

        public IActionResult TipoPago()
        {
            var modelo = new PagosFormModel
            {
                TipoDePagoList = new List<TipoDePago>()
            };

            modelo.TipoDePagoList = ObtenerTipoDePago();

            return View("TipoPago",modelo);
        }

        public IActionResult Pagos()
        {
            var modelo = new PagosFormModel
            {
                TipoDePagoList = new List<TipoDePago>(),
                Pagos = new PagosItemModel(),
                Detalle = new List<DetallePagoItemModel>()
            };

            modelo.Meses = ObtenerMeses();
            modelo.TipoDePagoList = ObtenerTipoDePago();

            return View("Pagos", modelo);
        }

        public IActionResult Historial()
        {
            var modelo = new PagosItemModel();
            return View("Historial",modelo);
        }

        private List<TipoDePago> ObtenerTipoDePago()
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);
          
            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "SELECT * FROM Administracion.TipoDePago WHERE EstaActivo = 1";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new TipoDePago
            {
                IdTipoDePago = x.Field<Int16>("IdTipoDePago"),
                CodigoInterno = x.Field<String>("CodigoInterno"),
                Descripcion = x.Field<String>("Descripcion"),
                Precio = Convert.ToDecimal(Decimal.Round(x.Field<Decimal>("Precio"), 2)),
                Mes = x.Field<Boolean>("Mes"),
                Anio = x.Field<Boolean>("Anio"),
                LlevaMes = x.Field<Boolean>("Mes") ? "Si" : "No",
                LleaAnio = x.Field<Boolean>("Anio") ? "Si" : "No"
            }).ToList();
        }
        public ActionResult Guardar(PagosFormModel model )
        {
            
            var listado = "";
            model.Pagos.SubTotal = model.Detalle.Sum(x => x.Precio);
           
            model.Pagos.Total =  model.Detalle.Sum(x => x.Total);
            model.Pagos.Descuento = model.Pagos.SubTotal - model.Pagos.Total;
            model.Pagos.IdUsuario = Convert.ToInt16(HttpContext.Request.Cookies["IdUsuario"]);

            string jsonString = JsonConvert.SerializeObject(model);

            String connectionString = Conexion;
            String JsonResultado = jsonString;
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "EXEC Administracion.prGuardarPago @JSON";
            cmd.Parameters.AddWithValue("@JSON", JsonResultado);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
               JsonResultado = ds.Tables[0].AsEnumerable().Select(h => h.Field<String>("Resultado")).FirstOrDefault();
            }

            Resultado resultado = JsonConvert.DeserializeObject<Resultado>(JsonResultado);
            Boolean Exito = resultado.Exito;
            String Descripcion = resultado.Descripcion;
            Int32 IdPagoGenerado = resultado.IdPago;

            cnn.Close();

            Alert(Descripcion, (Exito ? NotificationType.success : NotificationType.warning));

            if (Exito)
            {
                Carnet = model.Pagos.Carnet;
                IdPago = Convert.ToString(IdPagoGenerado);
                var msg = "<script language='javascript'>"+ "window.open(window.origin + \"/Pagos/rptPagoGenerado?Carnet="+Carnet+"&IdPago="+IdPago+"\", \"Pago Generado\", \"width=500,height=500,scrollbars=NO\")"+" </script>";
                TempData["popup"] = msg;
                return model.PermisoAdministrar ? RedirectToAction("Pagos") : RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Pagos", LlenarModelo(model));
            }
        }

        public List<String> ObtenerMeses()
        {
            var NombreMeses = DateTimeFormatInfo.CurrentInfo.MonthNames;

            Int32 InicioMes = 1;

            var Meses = NombreMeses.Where(w => !String.IsNullOrEmpty(w)).Select(x => new
            {
                //IdMes = InicioMes++,
                x = CultureInfo.CreateSpecificCulture("es-ES").TextInfo.ToTitleCase(x)
                //x = (char.ToUpper(x[0]) + x.Substring(1))
            }); 

            var listastring = new List<String>();
            foreach (var mes in Meses)
            {
                listastring.Add(mes.x.ToString());
            }

            return listastring;
        }

        public ActionResult DetallePago(DetallePagoItemModel model)
        {         
            return PartialView("DetallePago", model);
        }
        public ActionResult ObtenerEstudiante(string NumeroCarnet)
        {
            var listado = "";
            String connectionString = Conexion;
             String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "Administracion.prObtenerEstudiantePago @carnet";
            cmd.Parameters.AddWithValue("@carnet", NumeroCarnet);
    
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            var estudiante = ds.Tables[0].AsEnumerable().Select(x=> new { 
              Nombre = x.Field<String>("NombreCompleto"),
              Descuento = x.Field<byte>("Porcentaje")
            }).FirstOrDefault();
            
            
            return Json(estudiante) ;
        }
        public ActionResult ObtenerHistorial(PagosItemModel model)
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            var carnet = "";

            if (String.IsNullOrEmpty(model.Carnet))
            {
                carnet = "";
            }
            else
            {
                carnet = model.Carnet;
            }
            cmd.CommandText = "Administracion.prObtenerHistorialPago @FechaDesde,@FechaHasta,@Carnet";
            cmd.Parameters.AddWithValue("@FechaDesde", model.FechaDesde);
            cmd.Parameters.AddWithValue("@FechaHasta", model.FechaHasta);
            cmd.Parameters.AddWithValue("@Carnet", carnet);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            var Lista = ds.Tables[0].AsEnumerable().Select(x => new PagosItemModel
            {
                Carnet = x.Field<String>("Carnet"),
                NombreCompleto = x.Field<String>("NombreCompleto"),
                Descripcion = x.Field<String>("Descripcion"),
                Precio = Decimal.Round(x.Field<Decimal>("Precio"),2),
                Descuento = Decimal.Round(x.Field<Decimal>("Descuento"),0),
                Total = decimal.Round(x.Field<Decimal>("Total"), 2),
                Fecha = x.Field<DateTime>("FechaCreacion")
            }).ToList();

            return PartialView("DetalleHistorialPago", Lista);
        }

        public ActionResult ObtenerHistorialDetalle(PagosItemModel model)
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            var carnet = "";
            var FechaDesde = Convert.ToString(model.FechaDesde);
            var FechaHasta = Convert.ToString(model.FechaHasta);

            if (String.IsNullOrEmpty(model.Carnet))
            {
                carnet = "";
            }
            else
            {
                carnet = model.Carnet;
            }
            cmd.CommandText = "Administracion.prObtenerHistorialPago @FechaDesde,@FechaHasta,@Carnet";
            cmd.Parameters.AddWithValue("@FechaDesde", FechaDesde);
            cmd.Parameters.AddWithValue("@FechaHasta", FechaHasta);
            cmd.Parameters.AddWithValue("@Carnet", carnet);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            var Lista = ds.Tables[0].AsEnumerable().Select(x => new PagosItemModel
            {
                Carnet = x.Field<String>("Carnet"),
                NombreCompleto = x.Field<String>("NombreCompleto"),
                Descripcion = x.Field<String>("Descripcion"),
                Precio = Decimal.Round(x.Field<Decimal>("Precio"), 2),
                Descuento = Decimal.Round(x.Field<Decimal>("Descuento"), 0),
                Total = decimal.Round(x.Field<Decimal>("Total"), 2),
                Fecha = x.Field<DateTime>("FechaCreacion")
            }).ToList();

            return PartialView("DetalleHistorialPago", Lista);
        }
        public PagosFormModel LlenarModelo(PagosFormModel formModel)
        {
            formModel = formModel ?? new PagosFormModel();

            formModel.TipoDePagoList = ObtenerTipoDePago();

            formModel.Meses = ObtenerMeses();

            return formModel;
        }

        /*reporte*/

        public IActionResult rptPagoGenerado(String Carnet,String IdPago)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var filePath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\Reportes\\rptPagoGenerado.rdl";

            var _params = new Dictionary<string, string>
            {
                {"Carnet",Carnet },
                {"IdPago",IdPago }
            };

            var b = UniPromecys.Models.Reportes.RDL.Create(filePath, _params);
            return File(b, "application/pdf");
        }
    }
}
