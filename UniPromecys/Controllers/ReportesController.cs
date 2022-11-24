using AspNetCore.Reporting;
using Microsoft.AspNetCore.Mvc;
using UniPromecys.Models.Reportes;

namespace UniPromecys.Controllers
{
    public class ReportesController : Controller
    {
        public IActionResult rptEmpleados()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //var filePath = @"C:\Users\Paulino Zelaya\Desktop\UniPromecys\UniPromecys\wwwroot\Reportes\rptEmpleados.rdl";
            var filePath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\Reportes\\rptEmpleados.rdl";
            var local = new LocalReport(filePath);
            var rpt = local.Execute(RenderType.Pdf);
            return File(rpt.MainStream,"application/pdf");
        }

        public IActionResult rptEstudiantes()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var filePath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\Reportes\\rptEstudiantes.rdl";
            var b = UniPromecys.Models.Reportes.RDL.Create(filePath);
            return File(b, "application/pdf");
        }
    }
}
