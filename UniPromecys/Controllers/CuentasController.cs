using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using UniPromecys.Date;
using UniPromecys.Models;
using UniPromecys.Models.Asignatura;
using UniPromecys.Models.Cuentas;
using UniPromecys.Models.Direccion;
using UniPromecys.Models.Empleado;
using UniPromecys.Models.Estudiante;
using static UniPromecys.Models.AccionesControlador;
using static UniPromecys.Models.Enum;

namespace UniPromecys.Controllers
{
    public class CuentasController : BaseController
    {
        ObtenerDatos obtenerDatos = new ObtenerDatos();
        public string Conexion => obtenerDatos.Conexion();
        public IActionResult Administrar()
        {
            var modelo = new CuentasAdminModel()
            {
                ListadoCuentas = Listado()
            };

            return View("Administrar", modelo);
        }

        public IActionResult RegistrarCuenta()
        {
            var modelo = new CuentasFormModel()
            {
                CuentasItemModel = new CuentasItemModel(),
                modulosList = new List<Modulos>(),
                permisosList = new List<Permisos>(),
                empleadoList = new List<Models.Empleado.EmpleadoItemModel>(),
                DetalleCuenta = new List<DetalleRolCuentaItem>()
            };

            modelo.modulosList = ObtenerModulos();
            modelo.empleadoList = ListadoEmpleados();
            modelo.CuentasItemModel.Accion = Acciones.Nuevo.ToString();
            return View("RegistrarCuenta", modelo);
        }

        public CuentasFormModel LlenarModelo(CuentasFormModel formModel)
        {
            formModel = formModel ?? new CuentasFormModel();

            formModel.empleadoList = ListadoEmpleados();

            formModel.modulosList = ObtenerModulos();

            formModel.permisosList = new List<Permisos>();

            //formModel.DetalleCuenta = ObtenerDetalleRolCuenta();

            return formModel;
        }

        public IActionResult GuardarCuenta(CuentasFormModel cuentasFormModel)
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

                String JSONRol = JsonConvert.SerializeObject(cuentasFormModel);

                if (cuentasFormModel.CuentasItemModel.IdCuenta > 0)
                {
                    cmd.CommandText = "EXEC Cuenta.prEditarCuenta @JSONRol, @IdUsuario";
                }
                else
                {
                    cmd.CommandText = "EXEC Cuenta.prAñadirCuenta @JSONRol, @IdUsuario";
                }
                cmd.Parameters.AddWithValue("@JSONRol", JSONRol);
                cmd.Parameters.AddWithValue("@IdUsuario", Convert.ToInt32(IdUsuario));
                //cmd.Parameters.AddWithValue("@IdUsuario", );
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
                    return cuentasFormModel.PermisoAdministrar ? RedirectToAction("Administrar") : RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("RegistrarCuenta", LlenarModelo(cuentasFormModel));
                }

            }

            catch (Exception e)
            {
                String Descripcion = "Ha ocurrido un error, Contactar al administrador";
                ViewBag.Resultado = Descripcion;
                Alert(Descripcion, NotificationType.error);
                return View("RegistrarCuenta", LlenarModelo(cuentasFormModel));
            }
        }


        public IActionResult Editar(Int32 IdCuenta)
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                //var modelo = new CuentasItemModel();
                var modelo = new CuentasFormModel()
                {
                    CuentasItemModel = new CuentasItemModel(),
                    modulosList = new List<Modulos>(),
                    permisosList = new List<Permisos>(),
                    empleadoList = new List<EmpleadoItemModel>(),
                    DetalleCuenta = new List<DetalleRolCuentaItem>()
                };
                var Encontrado = Listado().FirstOrDefault(x => x.IdCuenta == IdCuenta);
                modelo.CuentasItemModel = Encontrado;
                modelo.modulosList = ObtenerModulos();
                modelo.permisosList = ObtenerPermisosList(modelo.IdModulo);
                modelo.empleadoList = ListadoEmpleados();
                modelo.DetalleCuenta = ObtenerDetalleRolCuenta(IdCuenta);
                modelo.CuentasItemModel.Accion = Acciones.Editar.ToString();
                return View("RegistrarCuenta", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        } 

        public List<DetalleRolCuentaItem> ObtenerDetalleRolCuenta(Int32 IdCuenta)
        {
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;

            cmd.CommandText = "SELECT M.IdModulo,M.Nombre AS Modulo,R.IdRol,R.Nombre AS Permiso FROM Cuenta.RolCuenta AS Rc\r\nINNER JOIN Cuenta.Rol AS R ON R.IdRol = Rc.IdRol\r\nINNER JOIN Administracion.Modulo AS M ON M.IdModulo = R.IdModulo\r\nWHERE Rc.IdCuenta = @IdCuenta AND Rc.EstaActivo = 1";
            cmd.Parameters.AddWithValue("@IdCuenta", IdCuenta);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new DetalleRolCuentaItem
            {
                IdModulo = x.Field<Int16>("IdModulo"),
                Modulo = x.Field<String>("Modulo"),
                IdPermiso = x.Field<Int16>("IdRol"),
                Permiso = x.Field<String>("Permiso")
            }).ToList();
        }

        public IActionResult Eliminar(Int32 IdCuenta)
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

                cmd.CommandText = "EXEC Cuenta.prEliminarCuenta @IdCuenta,@IdUsuario";
                cmd.Parameters.AddWithValue("@IdCuenta", IdCuenta);
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
            return RedirectToAction("Administrar");
        }

        private List<CuentasItemModel> Listado()
        {
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;

            cmd.CommandText = "SELECT * FROM Cuenta.fnObtenerCuentas()";
            //cmd.Parameters.AddWithValue("@UserName", model.Usuario);
            //cmd.Parameters.AddWithValue("@Password", model.Contrasena);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new CuentasItemModel
            {
                IdCuenta = x.Field<Int16>("IdCuenta"),
                Usuario = x.Field<String>("Usuario"),
                Contrasenia = x.Field<String>("Contrasena"),
                IdPersona = x.Field<Int16>("IdPersona"),
                NombreCompleto = x.Field<String>("NombreCompleto")
            }).ToList();

        }

        private List<Modulos> ObtenerModulos()
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "SELECT * FROM Administracion.Modulo";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new Modulos
            {
                IdModulo = x.Field<Int16>("IdModulo"),
                Nombre = x.Field<String>("Nombre")
            }).ToList();
        }

        [HttpGet]
        public IActionResult ObtenerPermisos(Int32 IdModulo)
        {
            var resultado = ObtenerPermisosList(IdModulo);

            return Json(resultado);
        }

        private List<Permisos> ObtenerPermisosList(Int32 IdModulo)
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "SELECT * FROM Cuenta.Rol WHERE IdModulo = @IdModulo";
            cmd.Parameters.AddWithValue("@IdModulo", IdModulo);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new Permisos
            {
                IdRol = x.Field<Int16>("IdRol"),
                Nombre = x.Field<String>("Nombre")
            }).ToList();
        }

        public List<EmpleadoItemModel> ListadoEmpleados()
        {
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;

            cmd.CommandText = "SELECT * FROM Administracion.fnObtenerEmpleados()";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new EmpleadoItemModel
            {
                IdEmpleado = x.Field<Int16>("IdEmpleado"),
                IdCargo = x.Field<Int16>("IdCargo"),
                Nombre = x.Field<String>("Nombre"),
                IdPersona = x.Field<Int16>("IdPersona"),
                PrimerNombre = x.Field<String>("PrimerNombre"),
                SegundoNombre = x.Field<String>("SegundoNombre"),
                PrimerApellido = x.Field<String>("PrimerApellido"),
                SegundoApellido = x.Field<String>("SegundoApellido"),
                NombreCompleto = x.Field<String>("NombreCompleto"),
                Edad = x.Field<Byte>("Edad"),
                IdGenero = x.Field<Int16>("IdGenero"),
                IdTipoIdentificacion = x.Field<Int16>("IdTipoIdentificacion"),
                Identificacion = x.Field<String>("Identificacion"),
                IdTipoContacto = x.Field<Int16>("IdTipoContacto"),
                Contacto = x.Field<Int32>("Contacto"),
                Correo = x.Field<String>("Correo"),
                IdNacionalidad = x.Field<Int16>("IdNacionalidad"),
                IdDepartamento = x.Field<Int16>("IdDepartamento"),
                IdMunicipio = x.Field<Int16>("IdMunicipio"),
                Direccion = x.Field<String>("Direccion"),
                CodigoPostal = x.Field<String>("CodigoPostal")
            }).ToList();
        }

        public IActionResult AgregarDetalle(DetalleRolCuentaItem model)
        {
            return PartialView("DetalleRolCuenta",model);
        }

    }
}
