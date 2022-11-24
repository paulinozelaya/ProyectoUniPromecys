using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using UniPromecys.Models.Estudiante;
using static UniPromecys.Models.AccionesControlador;
using static UniPromecys.Models.Enum;
using UniPromecys.Models.Asignatura;
using Newtonsoft.Json;
using UniPromecys.Models;
using UniPromecys.Models.Direccion;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.AspNetCore.Mvc;
using UniPromecys.Date;

namespace UniPromecys.Controllers
{
    public class EstudianteController : BaseController
    {
        ObtenerDatos obtenerDatos = new ObtenerDatos();
        public string Conexion => obtenerDatos.Conexion();
        private List<EstudianteItemModel> Listado()
        {
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;

            cmd.CommandText = "SELECT * FROM Estudiante.fnObtenerEstudiantes()";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new EstudianteItemModel
            {
                IdEstudiante = x.Field<Int16>("IdEstudiante"),
                IdPersona = x.Field<Int16>("IdPersona"),
                Carnet = x.Field<String>("Carnet"),
                IdSolvencia = x.Field<Int16>("IdSolvencia"),
                Solvencia = x.Field<String>("Solvencia"),
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

                var modelo = new EstudianteAdminModel
                {
                    ListadoEstudiantes = Listado()
                };

                return View("Administrar", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        }

        public IActionResult RegistrarEstudiante()
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                var modelo = new EstudianteFormModel
                {
                    Estudiante = new EstudianteItemModel(),

                    NacionalidadList = new List<Nacionalidad>(),
                    DeparamentoList = new List<Departamento>(),
                    MunicipioList = new List<Municipio>(),
                    SolvenciaList = new List<SolvenciaModel>()
                };

                modelo.NacionalidadList = ObtenerNacionalidad();
                modelo.SolvenciaList = ObtenerDescuento();
                modelo.Estudiante.Accion = Acciones.Nuevo.ToString();
                return View("RegistrarEstudiante", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");

            }
        }

        public EstudianteFormModel LlenarModelo(EstudianteFormModel formModel)
        {
            formModel = formModel ?? new EstudianteFormModel();

            formModel.NacionalidadList = ObtenerNacionalidad();

            formModel.DeparamentoList = formModel.IdNacionalidad > 0 ? ObtenerDepartamentoList(formModel.IdNacionalidad) : (formModel.IdNacionalidad <= 0) ? ObtenerDepartamentoList(formModel.Estudiante.IdNacionalidad) : new List<Departamento>();

            formModel.MunicipioList = formModel.IdDepartamento > 0 ? ObtenerMunicipioList(formModel.IdDepartamento) : (formModel.IdDepartamento <= 0) ? ObtenerMunicipioList(formModel.Estudiante.IdDepartamento) : new List<Municipio>();

            formModel.SolvenciaList = ObtenerDescuento();

            return formModel;
        }

        [HttpPost]
        public IActionResult GuardarEstudiante(EstudianteFormModel estudianteFormModel)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    return View("RegistrarEstudiante");
                //}
                var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
                String connectionString = Conexion;
                String JsonResultado = "";
                SqlConnection cnn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand();
                //DataTable dataTable = new DataTable();
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA;

                if (estudianteFormModel.Estudiante.IdEstudiante > 0)
                {
                    cmd.CommandText = "EXEC Estudiante.prEditarEstudiante @IdEstudiante, @Carnet, @IdSolvencia, @PrimerNombre, @SegundoNombre, @PrimerApellido, @SegundoApellido, @Edad, @IdGenero, @IdTipoIdentificacion, @Identificacion, @IdTipoContacto, @Contacto,@Correo, @IdUsuario,@IdMunicipio,@Direccion,@CodigoPostal";
                    cmd.Parameters.AddWithValue("@IdEstudiante", estudianteFormModel.Estudiante.IdEstudiante);
                }
                else
                {
                    cmd.CommandText = "EXEC Estudiante.prAñadirEstudiante @Carnet, @IdSolvencia, @PrimerNombre, @SegundoNombre, @PrimerApellido, @SegundoApellido, @Edad, @IdGenero, @IdTipoIdentificacion, @Identificacion, @IdTipoContacto, @Contacto,@Correo, @IdUsuario,@IdMunicipio,@Direccion,@CodigoPostal";
                }
                cmd.Parameters.AddWithValue("@Carnet", estudianteFormModel.Estudiante.Carnet);
                cmd.Parameters.AddWithValue("@IdSolvencia", estudianteFormModel.Estudiante.IdSolvencia);
                cmd.Parameters.AddWithValue("@PrimerNombre", estudianteFormModel.Estudiante.PrimerNombre);
                cmd.Parameters.AddWithValue("@SegundoNombre", String.IsNullOrEmpty(estudianteFormModel.Estudiante.SegundoNombre) ? "" : estudianteFormModel.Estudiante.SegundoNombre); ;
                cmd.Parameters.AddWithValue("@PrimerApellido", estudianteFormModel.Estudiante.PrimerApellido);
                cmd.Parameters.AddWithValue("@SegundoApellido", String.IsNullOrEmpty(estudianteFormModel.Estudiante.SegundoApellido) ? "" : estudianteFormModel.Estudiante.SegundoApellido);
                cmd.Parameters.AddWithValue("@Edad", estudianteFormModel.Estudiante.Edad);
                cmd.Parameters.AddWithValue("@IdGenero", estudianteFormModel.Estudiante.IdGenero);
                cmd.Parameters.AddWithValue("@IdTipoIdentificacion", estudianteFormModel.Estudiante.IdTipoIdentificacion);
                cmd.Parameters.AddWithValue("@Identificacion", estudianteFormModel.Estudiante.Identificacion);
                cmd.Parameters.AddWithValue("@IdTipoContacto", estudianteFormModel.Estudiante.IdTipoContacto);
                cmd.Parameters.AddWithValue("@Contacto", estudianteFormModel.Estudiante.Contacto);
                cmd.Parameters.AddWithValue("@Correo", estudianteFormModel.Estudiante.Correo);
                cmd.Parameters.AddWithValue("@IdUsuario", Convert.ToInt32(IdUsuario));
                cmd.Parameters.AddWithValue("@IdMunicipio", estudianteFormModel.Estudiante.IdMunicipio);
                cmd.Parameters.AddWithValue("@Direccion", estudianteFormModel.Estudiante.Direccion);
                cmd.Parameters.AddWithValue("@CodigoPostal", String.IsNullOrEmpty(estudianteFormModel.Estudiante.CodigoPostal) ? "" : estudianteFormModel.Estudiante.CodigoPostal);
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
                    return estudianteFormModel.Estudiante.PermisoAdministrar ? RedirectToAction("Administrar") : RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("RegistrarEstudiante", LlenarModelo(estudianteFormModel));
                }
            }
            catch (Exception e)
            {
                String Descripcion = "Ha ocurrido un error, Contactar al administrador";
                ViewBag.Resultado = Descripcion;
                Alert(Descripcion, NotificationType.error);
                return View("RegistrarEstudiante", LlenarModelo(estudianteFormModel));
            }
        }

        public IActionResult Editar(Int32 IdEstudiante)
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                var modelo = new EstudianteFormModel();
                var Encontrado = Listado().FirstOrDefault(x => x.IdEstudiante == IdEstudiante);
                modelo.Estudiante = Encontrado;
                modelo.Estudiante.Accion = Acciones.Editar.ToString();
                return View("RegistrarEstudiante", LlenarModelo(modelo));
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        }

        [HttpGet]
        public IActionResult Eliminar(Int32 IdEstudiante)
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

                cmd.CommandText = "EXEC Estudiante.prEliminarEstudiante @IdEstudiante,@IdUsuario";
                cmd.Parameters.AddWithValue("@IdEstudiante", IdEstudiante);
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

            var modelo = new EstudianteAdminModel
            {
                ListadoEstudiantes = Listado()
            };

            return RedirectToAction("Administrar");
        }

        private List<SolvenciaModel> ObtenerDescuento()
        {
            var listado = "";
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;
            cmd.CommandText = "SELECT * FROM Administracion.Solvencia WHERE EstaActivo = 1";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new SolvenciaModel
            {
                IdSolvencia = x.Field<Int16>("IdSolvencia"),
                CodigoInterno = x.Field<String>("CodigoInterno"),
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
            cmd.CommandText = "SELECT * FROM Geografia.Nacionalidad WHERE EstaActivo = 1";
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
            cmd.CommandText = "SELECT * FROM Geografia.Departamentos WHERE IdNacionalidad = @IdNacionalidad AND EstaActivo = 1";
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
            cmd.CommandText = "SELECT * FROM Geografia.Municipios WHERE IdDepartamento = @IdDepartamento AND EstaActivo = 1";
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
    }
}
