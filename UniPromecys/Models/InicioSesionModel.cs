using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UniPromecys.Models
{
    public class InicioSesionModel
    {
        //[Required(ErrorMessage = "Usuario Requerido")]
        public String? Usuario { get; set; }

        //[Required(ErrorMessage = "Contraseña Requerida")]
        public String? Contrasena { get; set; }

    }
}
