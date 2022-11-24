using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;
using UniPromecys.Date;
using UniPromecys.Models;

namespace UniPromecys.Controllers
{
    public class InicioSesionController : Controller
    {
        ObtenerDatos obtenerDatos = new ObtenerDatos();
        public string Conexion => obtenerDatos.Conexion();
        public IActionResult InicioSesion()
        {
            HttpContext.Response.Cookies.Delete("IdUsuario");
            HttpContext.Response.Cookies.Delete("Usuario");
            HttpContext.Response.Cookies.Delete("Cargo");
            return View();
        }

        public IActionResult CerrarSesion()
        {
            HttpContext.Response.Cookies.Delete("IdUsuario");
            return View("InicioSesion");
        }

        [HttpPost]
        public ActionResult IniciarSesion(InicioSesionModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("InicioSesion", model);
                }
                //String connectionString = "Data Source=localhost;Initial Catalog=UniPromecys;Integrated Security=True";
                String connectionString = Conexion;
                String JsonResultado = "";
                SqlConnection cnn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand();
                //DataTable dataTable = new DataTable();
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA;

                cmd.CommandText = "EXEC Cuenta.prInicioSesion @UserName,@Password";
                cmd.Parameters.AddWithValue("@UserName", model.Usuario);
                cmd.Parameters.AddWithValue("@Password", model.Contrasena);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cnn;
                sqlDA = new SqlDataAdapter(cmd);
                sqlDA.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    JsonResultado = ds.Tables[0].AsEnumerable().Select(h => h.Field<String>("Datos")).FirstOrDefault();
                }

                Resultado resultado = JsonConvert.DeserializeObject<Resultado>(JsonResultado);
                Boolean Exito = resultado.Exito;
                String Descripcion = resultado.Descripcion;

                //Permisos = JsonConvert.SerializeObject(Permisos);
                

                cnn.Close();

                if (Exito)
                {
                    Int32 IdUsuario = resultado.IdUsuario;
                    String NombreUsuario = resultado.NombreUsuario;
                    String Cargo = resultado.Cargo;
                    String Permisos = resultado.Permisos;
                    HttpContext.Response.Cookies.Append("IdUsuario", Convert.ToString(IdUsuario));
                    HttpContext.Response.Cookies.Append("Usuario", (!String.IsNullOrEmpty(NombreUsuario)?NombreUsuario:""));
                    HttpContext.Response.Cookies.Append("Cargo", (!String.IsNullOrEmpty(Cargo) ? Cargo : ""));
                    HttpContext.Response.Cookies.Append("Permisos", (!String.IsNullOrEmpty(Permisos) ? Permisos : ""));
                    ViewBag.Permisos = Permisos;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Resultado = Descripcion;
                    return View("InicioSesion", model);
                }
                //  return RedirectToAction("Principal", "Principal", new { SesionActiva = true });
            }
            catch (Exception e)
            {
                String Descripcion = "Ha ocurrido un error";
                ViewBag.Resultado = Descripcion;
                return View("InicioSesion",model);
            }
        }
    }
}