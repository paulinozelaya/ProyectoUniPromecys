namespace UniPromecys.Models.Empleado
{
    public class EmpleadoItemModel : FormViewModel
    {
        public Int32 IdEmpleado { get; set; }

        public Int32 IdCargo { get; set; }

        public String Nombre { get; set; }

        public Int32 IdPersona { get; set; }

        public String Carnet { get; set; }

        public Int32 IdSolvencia { get; set; }  

        public String Solvencia { get; set; }

        public String PrimerNombre { get; set; }

        public String SegundoNombre { get; set; }

        public String PrimerApellido { get; set; }

        public String SegundoApellido { get; set; }

        public String NombreCompleto { get; set; }

        public Int32 Edad { get; set; }

        public Int32 IdGenero { get; set; }

        public Int32 IdTipoIdentificacion { get; set; }

        public String Identificacion { get; set; }

        public Int32 IdTipoContacto { get; set; }
        public Int32 Contacto { get; set; } 

        public String Correo { get; set; }
        public Int32 IdNacionalidad { get; set; }
        public Int32 IdDepartamento { get; set; }

        public Int32 IdMunicipio { get; set; }

        public String Direccion { get; set; }

        public String CodigoPostal { get; set; }
    }
}
