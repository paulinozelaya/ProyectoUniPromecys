namespace UniPromecys.Date
{
    public class ObtenerDatos
    {
        public String Conexion()
        {
            //String Conexion = "server=localhost ; database=UniPromecys ; integrated security = true";
            String Conexion = "server=unipromecys.cw4johhaaqj3.us-east-1.rds.amazonaws.com ; database=UniPromecys;user = admin; password= Isela170599 ; integrated security = false";
            return Conexion;
        }
    }
}
