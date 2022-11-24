using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UniPromecys.Models.Asignatura
{
    public class AsignaturaItemModel : FormViewModel
    {
        public Int32 IdAsignatura { get; set; }
        public string? Nombre { get; set; }

        public string? CodigoInterno { get; set; }

        public Int32 ? Creditos { get; set; }

        public string? Descripcion { get; set; } 

        public DateTime? FechaCreacion { get; set; }
    }
}
