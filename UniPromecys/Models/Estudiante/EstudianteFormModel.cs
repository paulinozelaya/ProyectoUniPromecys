using UniPromecys.Models.Direccion;

namespace UniPromecys.Models.Estudiante
{
    public class EstudianteFormModel
    {
        public EstudianteItemModel Estudiante { get; set; }
        public Int32 IdNacionalidad { get; set; }
        public Int32 IdDepartamento { get; set; }

        public Int32 IdUsuario { get; set; }
        public List<Nacionalidad> NacionalidadList { get; set; }

        public List<Departamento> DeparamentoList { get; set; }

        public List<Municipio> MunicipioList { get; set; }

        public List<SolvenciaModel> SolvenciaList { get; set; }
    }
}
