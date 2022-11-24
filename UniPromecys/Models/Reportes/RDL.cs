﻿namespace UniPromecys.Models.Reportes
{
    public class RDL
    {
        public static byte[] Create(string filepath)
        {
            return RDL.Create(filepath, null, null, null);
        }
        public static byte[] Create(string filepath, Dictionary<string, string> @params = null)
        {
            return RDL.Create(filepath, null, @params, null);
        }
        public static byte[] Create(string filepath, Dictionary<string, object> source = null)
        {
            return RDL.Create(filepath, null, null, source);
        }
        public static byte[] Create(string filepath, Dictionary<string, string> @params = null, Dictionary<string, object> source = null)
        {
            return RDL.Create(filepath, null, @params, source);
        }
        public static byte[] Create(string filepath, string connectionstring = null, Dictionary<string, string> @params = null, Dictionary<string, object> source = null)
        {
            byte[] result = null;
            var fi = new System.IO.FileInfo(filepath);
            if (fi.Exists)
            {
                string pathtemp = System.IO.Path.GetTempPath() + @"AspNetCore.Reporting.RDL\";
                string filepathtemp = pathtemp + fi.Name;
                //string filepathtemp = System.IO.Path.GetTempFileName();
                if (!System.IO.Directory.Exists(pathtemp))
                {
                    System.IO.Directory.CreateDirectory(pathtemp);
                }
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    if (string.IsNullOrEmpty(connectionstring) || string.IsNullOrWhiteSpace(connectionstring))
                    {
                        filepathtemp = filepath;
                    }
                    else
                    {
                        var b = System.IO.File.ReadAllBytes(filepath);
                        var ms = new System.IO.MemoryStream(b);
                        var xDoc = System.Xml.Linq.XDocument.Load(filepath);
                        var CElement = xDoc.Root.Descendants($"{{http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition}}ConnectString").SingleOrDefault();
                        if (CElement != null)
                        {
                            CElement.Value = connectionstring;
                            xDoc.Save(filepathtemp);
                        }
                    }
                    var localReport = new AspNetCore.Reporting.LocalReport(filepathtemp);
                    if (source != null && source.Count > 0)
                    {
                        foreach (var item in source)
                        {
                            localReport.AddDataSource(item.Key, item.Value);
                        }
                    }
                    var types = typeof(AspNetCore.Reporting.LocalReport);
                    var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
                    var field = types.GetField("localReport", flags);
                    var internalLocalReport = field.GetValue(localReport);
                    var typeinternal = internalLocalReport.GetType();
                    var propinternal = typeinternal.GetProperty("DisplayName", flags);
                    var propEnableExternalImages = typeinternal.GetProperty("EnableExternalImages", flags);
                    propEnableExternalImages.SetValue(internalLocalReport, true);

                    //var propinternalDisplay = typeinternal.GetProperty("DisplayNameForUse", flags);
                    //propinternal.SetValue(internalLocalReport, "NEW NAME");
                    //propinternalDisplay.SetValue(internalLocalReport, "NEW NAME");//No se puede escribir una propiedad readonly
                    var rr = localReport.Execute(AspNetCore.Reporting.RenderType.Pdf, 1, @params);
                    result = rr.MainStream;
                }
                catch (Exception ex)
                {
                    var _ = ex.Message;
                }
                finally
                {
                    if (!filepath.Equals(filepathtemp) && System.IO.File.Exists(filepathtemp))
                    {
                        System.IO.File.Delete(filepathtemp);
                    }
                }
            }
            return result;
        }
    }

}
