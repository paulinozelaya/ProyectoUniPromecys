using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using UniPromecys.Models.Empleado;
using static UniPromecys.Models.AccionesControlador;
using static UniPromecys.Models.Enum;
using UniPromecys.Models.Asignatura;
using Newtonsoft.Json;
using UniPromecys.Models;
using UniPromecys.Models.Direccion;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc;
using UniPromecys.Models.Estudiante;
using System.Reflection;
using UniPromecys.Date;

namespace UniPromecys.Controllers
{
    public class EmpleadoController : BaseController
    {
        ObtenerDatos obtenerDatos = new ObtenerDatos();
        public string Conexion => obtenerDatos.Conexion();
        public List<EmpleadoItemModel> Listado()
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
        public IActionResult Administrar()
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {

                var modelo = new EmpleadoAdminModel
                {
                    ListadoEmpleados = Listado()
                };

                return View("Administrar", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        }

        public IActionResult RegistrarEmpleado()
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                var modelo = new EmpleadoFormModel
                {
                    Empleado = new EmpleadoItemModel(),

                    NacionalidadList = new List<Nacionalidad>(),
                    DeparamentoList = new List<Departamento>(),
                    MunicipioList = new List<Municipio>(),
                    CargoList = new List<CargoModel>()
                };

                modelo.NacionalidadList = ObtenerNacionalidad();
                modelo.CargoList = ObtenerCargo();
                modelo.Empleado.Accion = Acciones.Nuevo.ToString();
                return View("RegistrarEmpleado", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");

            }
        }

        public EmpleadoFormModel LlenarModelo(EmpleadoFormModel formModel)
        {
            formModel = formModel ?? new EmpleadoFormModel();

            formModel.NacionalidadList = ObtenerNacionalidad();

            formModel.DeparamentoList = formModel.IdNacionalidad > 0 ? ObtenerDepartamentoList(formModel.IdNacionalidad) : (formModel.IdNacionalidad <= 0) ? ObtenerDepartamentoList(formModel.Empleado.IdNacionalidad) : new List<Departamento>();

            formModel.MunicipioList = formModel.IdDepartamento > 0 ? ObtenerMunicipioList(formModel.IdDepartamento) : (formModel.IdDepartamento <= 0) ? ObtenerMunicipioList(formModel.Empleado.IdDepartamento) : new List<Municipio>();

            formModel.CargoList = ObtenerCargo();

            return formModel;
        }

        [HttpPost]
        public IActionResult GuardarEmpleado(EmpleadoFormModel EmpleadoFormModel)
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

                if (EmpleadoFormModel.Empleado.IdEmpleado > 0)
                {
                    cmd.CommandText = "EXEC Administracion.prEditarEmpleado @IdEmpleado, @IdCargo, @PrimerNombre, @SegundoNombre, @PrimerApellido, @SegundoApellido, @Edad, @IdGenero, @IdTipoIdentificacion, @Identificacion, @IdTipoContacto, @Contacto,@Correo, @IdUsuario,@IdMunicipio,@Direccion,@CodigoPostal";
                    cmd.Parameters.AddWithValue("@IdEmpleado", EmpleadoFormModel.Empleado.IdEmpleado);
                }
                else
                {
                    cmd.CommandText = "EXEC Administracion.prAñadirEmpleado @IdCargo,@PrimerNombre, @SegundoNombre, @PrimerApellido, @SegundoApellido, @Edad, @IdGenero, @IdTipoIdentificacion, @Identificacion, @IdTipoContacto, @Contacto,@Correo, @IdUsuario,@IdMunicipio,@Direccion,@CodigoPostal";
                }
                cmd.Parameters.AddWithValue("@IdCargo", EmpleadoFormModel.Empleado.IdCargo);
                cmd.Parameters.AddWithValue("@PrimerNombre", EmpleadoFormModel.Empleado.PrimerNombre);
                cmd.Parameters.AddWithValue("@SegundoNombre", String.IsNullOrEmpty(EmpleadoFormModel.Empleado.SegundoNombre) ? "" : EmpleadoFormModel.Empleado.SegundoNombre);
                cmd.Parameters.AddWithValue("@PrimerApellido", String.IsNullOrEmpty(EmpleadoFormModel.Empleado.PrimerApellido) ? "" : EmpleadoFormModel.Empleado.PrimerApellido);
                cmd.Parameters.AddWithValue("@SegundoApellido", EmpleadoFormModel.Empleado.SegundoApellido);
                cmd.Parameters.AddWithValue("@Edad", EmpleadoFormModel.Empleado.Edad);
                cmd.Parameters.AddWithValue("@IdGenero", EmpleadoFormModel.Empleado.IdGenero);
                cmd.Parameters.AddWithValue("@IdTipoIdentificacion", EmpleadoFormModel.Empleado.IdTipoIdentificacion);
                cmd.Parameters.AddWithValue("@Identificacion", EmpleadoFormModel.Empleado.Identificacion);
                cmd.Parameters.AddWithValue("@IdTipoContacto", EmpleadoFormModel.Empleado.IdTipoContacto);
                cmd.Parameters.AddWithValue("@Contacto", EmpleadoFormModel.Empleado.Contacto);
                cmd.Parameters.AddWithValue("@Correo", EmpleadoFormModel.Empleado.Correo);
                cmd.Parameters.AddWithValue("@IdUsuario", Convert.ToInt32(IdUsuario));
                cmd.Parameters.AddWithValue("@IdMunicipio", EmpleadoFormModel.Empleado.IdMunicipio);
                cmd.Parameters.AddWithValue("@Direccion", EmpleadoFormModel.Empleado.Direccion);
                cmd.Parameters.AddWithValue("@CodigoPostal", String.IsNullOrEmpty(EmpleadoFormModel.Empleado.CodigoPostal) ? "" : EmpleadoFormModel.Empleado.CodigoPostal);
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
                    return EmpleadoFormModel.Empleado.PermisoAdministrar ? RedirectToAction("Administrar") : RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("RegistrarEmpleado", LlenarModelo(EmpleadoFormModel));
                }
            }
            catch (Exception e)
            {
                String Descripcion = "Ha ocurrido un error, Contactar al administrador";
                ViewBag.Resultado = Descripcion;
                Alert(Descripcion, NotificationType.error);
                return View("RegistrarEmpleado", LlenarModelo(EmpleadoFormModel));
            }
        }

        public IActionResult Editar(Int32 IdEmpleado)
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                var modelo = new EmpleadoFormModel();
                var Encontrado = Listado().FirstOrDefault(x => x.IdEmpleado == IdEmpleado);
                modelo.Empleado = Encontrado;
                modelo.Empleado.Accion = Acciones.Editar.ToString();
                return View("RegistrarEmpleado", LlenarModelo(modelo));
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        }

        [HttpGet]
        public IActionResult Eliminar(Int32 IdEmpleado)
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

                cmd.CommandText = "EXEC Administracion.prEliminarEmpleado @IdEmpleado,@IdUsuario";
                cmd.Parameters.AddWithValue("@IdEmpleado", IdEmpleado);
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

            var modelo = new EmpleadoAdminModel
            {
                ListadoEmpleados = Listado()
            };

            return RedirectToAction("Administrar");
        }

        private List<CargoModel> ObtenerCargo()
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "SELECT * FROM Administracion.Cargo";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new CargoModel
            {
                IdCargo = x.Field<Int16>("IdCargo"),
                CodigoInterno = x.Field<String>("CodigoInterno"),
                Nombre = x.Field<String>("Nombre"),
                Descripcion = x.Field<String>("Descripcion")
            }).ToList();
        }

        private List<Nacionalidad> ObtenerNacionalidad()
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "SELECT * FROM Geografia.Nacionalidad";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new Nacionalidad
            {
                IdNacionalidad = x.Field<Int16>("IdNacionalidad"),
                CodigoInterno = x.Field<String>("CodigoInterno"),
                Pais = x.Field<String>("Pais"),
                Descripcion = x.Field<String>("Descripcion")
            }).ToList();
        }

        [HttpGet]
        public IActionResult ObtenerDepartamento(Int32 IdNacionalidad)
        {
            var resultado = ObtenerDepartamentoList(IdNacionalidad);

            return Json(resultado);
        }
        [HttpGet]
        public IActionResult ObtenerMunicipio(Int32 IdDepartamento)
        {

            var resultado = ObtenerMunicipioList(IdDepartamento);

            return Json(resultado);
        }

        private List<Departamento> ObtenerDepartamentoList(Int32 IdNacionalidad)
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "SELECT * FROM Geografia.Departamentos WHERE IdNacionalidad = @IdNacionalidad";
            cmd.Parameters.AddWithValue("@IdNacionalidad", IdNacionalidad);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new Departamento
            {
                IdDepartamento = x.Field<byte>("IdDepartamento"),
                Nombre = x.Field<String>("Nombre")
            }).ToList();
        }

        private List<Municipio> ObtenerMunicipioList(Int32 IdDepartamento)
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "SELECT * FROM Geografia.Municipios WHERE IdDepartamento = @IdDepartamento";
            cmd.Parameters.AddWithValue("@IdDepartamento", IdDepartamento);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new Municipio
            {
                IdMunicipio = x.Field<byte>("IdMunicipio"),
                Nombre = x.Field<String>("Nombre")
            }).ToList();
        }

        public IActionResult AdministrarCargo()
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                return View("AdministrarCargo", ObtenerCargo());
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        }

        public IActionResult RegistrarCargo()
        {
            var modelo = new CargoModel();
            modelo.Accion = Acciones.Nuevo.ToString();
            return View("RegistrarCargo", modelo);
        }

        public IActionResult EditarCargo(Int32 IdCargo)
        {
            var modelo = new CargoModel();
            var Encontrado = ObtenerCargo().FirstOrDefault(x => x.IdCargo == IdCargo);
            modelo = Encontrado;
            modelo.Accion = Acciones.Editar.ToString();
            return View("RegistrarCargo", modelo);
        }

        public IActionResult EliminarCargo(Int32 IdCargo)
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

                cmd.CommandText = "EXEC Administracion.prEliminarCargo @IdCargo,@IdUsuario";
                cmd.Parameters.AddWithValue("@IdEmpleado", IdCargo);
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

            var modelo = new EmpleadoAdminModel
            {
                ListadoEmpleados = Listado()
            };

            return RedirectToAction("AdministrarCargo");
        }

        public IActionResult GuardarCargo(CargoModel cargoModel)
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

                if (cargoModel.IdCargo > 0)
                {
                    cmd.CommandText = "EXEC Administracion.prEditarCargo @IdCargo,@CodigoInterno,@Nombre,@Descripcion,@IdUsuario";
                    cmd.Parameters.AddWithValue("@IdCargo", cargoModel.IdCargo);
                }
                else
                {
                    cmd.CommandText = "EXEC Administracion.prAñadirCargo @CodigoInterno,@Nombre,@Descripcion,@IdUsuario";
                }
                cmd.Parameters.AddWithValue("@CodigoInterno", cargoModel.CodigoInterno);
                cmd.Parameters.AddWithValue("@Nombre", cargoModel.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", cargoModel.Descripcion);
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
                    return cargoModel.PermisoAdministrar ? RedirectToAction("Pagos") : RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("RegistrarCargo", cargoModel);
                }
            }
            catch (Exception e)
            {
                String Descripcion = "Ha ocurrido un error, Contactar al administrador";
                ViewBag.Resultado = Descripcion;
                Alert(Descripcion, NotificationType.error);
                return View("RegistrarCargo", cargoModel);
            }
        }
    }
}
