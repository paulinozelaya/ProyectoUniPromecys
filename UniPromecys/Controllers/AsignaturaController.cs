using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniPromecys.Models.Asignatura;
using static UniPromecys.Models.AccionesControlador;
using static UniPromecys.Models.Enum;
using System.Security.Cryptography;
using Newtonsoft.Json;
using UniPromecys.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using UniPromecys.Models.Estudiante;
using UniPromecys.Date;

namespace UniPromecys.Controllers
{
    public class AsignaturaController : BaseController
    {
        ObtenerDatos obtenerDatos = new ObtenerDatos();
        public string Conexion => obtenerDatos.Conexion();
        private List<AsignaturaItemModel> Listado()
        {
            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;

            cmd.CommandText = "SELECT * FROM Estudiante.Asignatura";
            //cmd.Parameters.AddWithValue("@UserName", model.Usuario);
            //cmd.Parameters.AddWithValue("@Password", model.Contrasena);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            return ds.Tables[0].AsEnumerable().Select(x => new AsignaturaItemModel
            {
                IdAsignatura = x.Field<Int16>("IdAsignatura"),
                CodigoInterno = x.Field<String>("CodigoInterno"),
                Nombre = x.Field<String>("Nombre"),
                Descripcion = x.Field<String>("Descripcion"),
                Creditos = x.Field<Byte>("Creditos"),
                FechaCreacion = x.Field<DateTime>("FechaCreacion")
            }).ToList();

        }
        public IActionResult Administrar()
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                var modelo = new AsignaturaAdminModel
                {
                    ListadoAsignaturas = Listado()
                };

                return View("Administrar", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion","InicioSesion");
            }
            

        }

        public IActionResult RegistrarAsignatura()
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {
                var modelo = new AsignaturaItemModel();
                modelo.Accion = Acciones.Nuevo.ToString();
                return View("RegistrarAsignatura", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        }

        [HttpPost]
        public IActionResult GuardarAsignatura(AsignaturaItemModel asignaturaItemModel)
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

                if (asignaturaItemModel.IdAsignatura > 0)
                {
                    cmd.CommandText = "EXEC Estudiante.prEditarAsignatura @IdAsignatura, @CodigoInterno, @Nombre, @Descripcion, @Creditos, @IdUsuario";
                    cmd.Parameters.AddWithValue("@IdAsignatura", asignaturaItemModel.IdAsignatura);
                }
                else
                {
                    cmd.CommandText = "EXEC Estudiante.prAñadirAsignatura @CodigoInterno, @Nombre, @Descripcion, @Creditos, @IdUsuario";
                }
                cmd.Parameters.AddWithValue("@CodigoInterno", asignaturaItemModel.CodigoInterno);
                cmd.Parameters.AddWithValue("@Nombre", asignaturaItemModel.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", asignaturaItemModel.Descripcion);
                cmd.Parameters.AddWithValue("@Creditos", asignaturaItemModel.Creditos);
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
                    return asignaturaItemModel.PermisoAdministrar ? RedirectToAction("Administrar") : RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("RegistrarAsignatura", asignaturaItemModel);
                }

            }
            catch (Exception e)
            {
                String Descripcion = "Ha ocurrido un error, Contactar al administrador";
                ViewBag.Resultado = Descripcion;
                Alert(Descripcion, NotificationType.error);
                return View("RegistrarAsignatura", asignaturaItemModel);
            }
        }

        public IActionResult Editar(Int32 IdAsignatura)
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario)>0)
            {
                var modelo = new AsignaturaItemModel();
                var Encontrado = Listado().FirstOrDefault(x => x.IdAsignatura == IdAsignatura);
                modelo = Encontrado;
                modelo.Accion = Acciones.Editar.ToString();
                return View("RegistrarAsignatura", modelo);
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        }

        [HttpGet]
        public IActionResult Eliminar(Int32 IdAsignatura)
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

                cmd.CommandText = "EXEC Estudiante.prEliminarAsignatura @IdAsignatura,@IdUsuario";
                cmd.Parameters.AddWithValue("@CodigoInterno", IdAsignatura);
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

            var modelo = new AsignaturaAdminModel
            {
                ListadoAsignaturas = Listado()
            };

            return RedirectToAction("Administrar");
        }
    }
}