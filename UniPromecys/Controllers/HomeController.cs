using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniPromecys.Date;
using UniPromecys.Models;
using UniPromecys.Models.Cuentas;
using UniPromecys.Models.Estudiante;

namespace UniPromecys.Controllers
{
    public class HomeController : Controller
    {
        ObtenerDatos obtenerDatos = new ObtenerDatos();
        public string Conexion => obtenerDatos.Conexion();
        public IActionResult Index()
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];
            if (Convert.ToInt32(IdUsuario) > 0)
            {

                return View();
            }
            else
            {
                return RedirectToAction("InicioSesion", "InicioSesion");
            }
        }
        public IActionResult ObtenerPermisos()
        {
            var IdUsuario = HttpContext.Request.Cookies["IdUsuario"];

            String connectionString = Conexion;
            String JsonResultado = "";
            SqlConnection cnn = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand();
            //DataTable dataTable = new DataTable();
            DataSet ds = new DataSet();
            SqlDataAdapter sqlDA;

            cmd.CommandText = "(SELECT R.IdRol, R.CodigoInterno, M.IdModulo, M.Nombre\r\n FROM   Cuenta.RolCuenta                 AS RC\r\n        INNER JOIN Cuenta.Rol            AS R\r\n            ON R.IdRol = RC.IdRol\r\n        INNER JOIN Administracion.Modulo AS M\r\n            ON M.IdModulo = R.IdModulo\r\n WHERE  RC.IdCuenta = @IdCuenta\r\n        AND RC.EstaActivo = 1);";
            cmd.Parameters.AddWithValue("@IdCuenta", IdUsuario);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            sqlDA = new SqlDataAdapter(cmd);
            sqlDA.Fill(ds);

            var obj = ds.Tables[0].AsEnumerable().Select(x => new
            {
                IdRol = x.Field<Int16>("IdRol"),
                CodigoInterno = x.Field<String>("CodigoInterno"),
                IdModulo = x.Field<Int16>("IdModulo"),
                Modulo = x.Field<String>("Nombre")
            }).ToList();
            
            return Json(obj);
        }

    }
}
