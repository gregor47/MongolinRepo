using CertificadosMaster.Data;
using ExcelDataReader;
using GemBox.Spreadsheet;
using Newtonsoft.Json.Linq;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CertificadosMaster.Controllers
{
    public class CertificadosController : Controller
    {
        private readonly PersonalEntities _context;
        public CertificadosController()
        {
            //_context = context;
        }
        public ActionResult Index()
        {
            return View();
        }
        #region CertificadoFinde
        [HttpPost]
        public async Task<JObject> GenerarFindeAño(HttpPostedFileBase inputFile)
        {
            PersonalEntities _context = new PersonalEntities();
            JObject result = new JObject();

            string NombreArchivoFin = "DataIngresosFinde" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string rutaUploads = @"C:\Certificados\FindeAño";
            var fullFilePath = Path.Combine(rutaUploads, NombreArchivoFin);
            inputFile.SaveAs(fullFilePath);

            List<DataFinAño> list = new List<DataFinAño>();
            using (var stream = System.IO.File.Open(fullFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            if (reader.Depth >= 1)
                            {
                                try
                                {
                                    DataFinAño data = new DataFinAño()
                                    {
                                        Nombre = reader.GetValue(0).ToString(),
                                        Cargo = reader.GetValue(1).ToString(),
                                        Identificacion = reader.GetValue(2).ToString(),
                                        Estado = false
                                    };
                                    list.Add(data);
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    } while (reader.NextResult());
                }
            }
            _context.DataFinAño.AddRange(list);
            await _context.SaveChangesAsync();

            int numero = await ConstruirExcel(list);

            result.Add("Codigo", "01");
            result.Add("Exitoso", "Se crearon " + numero + " Archivos.");

            return result;
        }
        private async Task<int> ConstruirExcel(List<DataFinAño> listingre)
        {
            PersonalEntities _context = new PersonalEntities();
            int enviados = 0;
            try
            {
                string Modelo = "ModeloFindeAño.xlsx";
                string rutaUploads = @"C:\Certificados\";
                string EndPath = Path.Combine(rutaUploads, "auxiliar.xlsx");
                string OutPutPath = string.Empty;
                var fullFilePath = Path.Combine(rutaUploads, Modelo);
                List<DataFinAño> list = listingre;
                string ruta = rutaUploads + $@"\Cargue{DateTime.Now.ToString("yyyyMMdd")}";
                if (!Directory.Exists(ruta))
                {
                    DirectoryInfo di = Directory.CreateDirectory(ruta);
                }
                //Initialize application
                foreach (DataFinAño item in list)
                {
                    try
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            //Initialize application
                            IApplication application = excelEngine.Excel;
                            OutPutPath = Path.Combine(rutaUploads + $"/Cargue{DateTime.Now.ToString("yyyyMMdd")}", item.Identificacion.Trim() + ".pdf");
                            //Open existing workbook with data entered
                            var stream = System.IO.File.Open(fullFilePath, FileMode.Open, FileAccess.ReadWrite);
                            Stream fileStream = stream;
                            IWorkbook workbook = application.Workbooks.Open(fileStream, ExcelOpenType.Automatic);
                            IWorksheet worksheet = workbook.Worksheets[0];
                            worksheet.EnableSheetCalculations();
                            Stream nruta = System.IO.File.Create(EndPath);
                            //Add text data
                            try
                            {
                                worksheet.Range["B7"].Text = item.Nombre;
                                worksheet.Range["D2"].Text = DateTime.Now.Year.ToString();
                                worksheet.Range["G36"].Text = item.Cargo;
                                worksheet.Range["H37"].Text = item.Identificacion;
                                workbook.SaveAs(nruta);
                                workbook.Close();
                                fileStream.Close();
                                excelEngine.Dispose();
                                nruta.Close();
                                SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                                var workbooks = ExcelFile.Load(EndPath);
                                workbooks.Worksheets.Remove(1);
                                var saveOptions = new PdfSaveOptions();
                                saveOptions.SelectionType = SelectionType.EntireFile;
                                workbooks.Save(OutPutPath, saveOptions);
                                item.Estado = true;
                            }
                            catch (Exception ex)
                            {
                                workbook.Close();
                                fileStream.Close();
                                excelEngine.Dispose();
                                nruta.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return enviados;
        }
        #endregion

        #region CertificadoMatricula
        [HttpPost]
        public async Task<JObject> GenerarMatriculado(HttpPostedFileBase inputFile)
        {
            PersonalEntities _context = new PersonalEntities();
            JObject result = new JObject();

            string NombreArchivoFin = "DataIngresosMatri" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string rutaUploads = @"C:\Certificados\Matriculados";
            var fullFilePath = Path.Combine(rutaUploads, NombreArchivoFin);
            inputFile.SaveAs(fullFilePath);

            List<CertificadoMatricula> list = new List<CertificadoMatricula>();
            using (var stream = System.IO.File.Open(fullFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            if (reader.Depth >= 1)
                            {
                                try
                                {
                                    CertificadoMatricula data = new CertificadoMatricula()
                                    {
                                        Nombre = reader.GetValue(0).ToString(),
                                        Documento = reader.GetValue(1).ToString(),
                                        TipoDocumento = reader.GetValue(2).ToString(),
                                        Grado = reader.GetValue(3).ToString(),
                                        Educacion = Educacion(reader.GetValue(3).ToString())
                                    };
                                    list.Add(data);
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    } while (reader.NextResult());
                }
            }
            _context.CertificadoMatricula.AddRange(list);
            await _context.SaveChangesAsync();

            int numero = await ConstruirExcelMatricula(list);

            result.Add("Codigo", "01");
            result.Add("Exitoso", "Se crearon " + numero + " Archivos.");

            return result;
        }
        public string Educacion(string grado)
        {
            string educacion = string.Empty;
            switch (grado)
            {
                case "TRANSICION":
                    educacion = "TRANSICIÓN*PRE-ESCOLAR";
                    break;
                case "1":
                    educacion = "PRIMERO*BÁSICA PRIMARIA";
                    break;
                case "2":
                    educacion = "SEGUNDO*BÁSICA PRIMARIA";
                    break;
                case "3":
                    educacion = "TERCERO*BÁSICA PRIMARIA";
                    break;
                case "4":
                    educacion = "CUARTO*BÁSICA PRIMARIA";
                    break;
                case "5":
                    educacion = "QUINTO*BÁSICA PRIMARIA";
                    break;
                case "6":
                    educacion = "SEXTO*BÁSICA SECUNDARIA";
                    break;
                case "7":
                    educacion = "SÉPTIMO*BÁSICA SECUNDARIA";
                    break;
                case "8":
                    educacion = "OCTAVO*BÁSICA SECUNDARIA";
                    break;
                case "9":
                    educacion = "NOVENO*BÁSICA SECUNDARIA";
                    break;
                case "10":
                    educacion = "DÉCIMO*MEDIA ACADÉMICA";
                    break;
                case "11":
                    educacion = "UNDÉCIMO*MEDIA ACADÉMICA";
                    break;
            }
            return educacion;
        }
        private async Task<int> ConstruirExcelMatricula(List<CertificadoMatricula> listingre)
        {
            PersonalEntities _context = new PersonalEntities();
            int enviados = 0;
            try
            {
                string Modelo = "ModeloMatricula.xlsx";
                string rutaUploads = @"C:\Certificados\";
                string EndPath = Path.Combine(rutaUploads, "auxiliar2.xlsx");
                string OutPutPath = string.Empty;
                var fullFilePath = Path.Combine(rutaUploads, Modelo);
                List<CertificadoMatricula> list = listingre;
                string ruta = rutaUploads + $@"\CargueMatricula{DateTime.Now.ToString("yyyyMMdd")}";
                if (!Directory.Exists(ruta))
                {
                    DirectoryInfo di = Directory.CreateDirectory(ruta);
                }
                //Initialize application
                foreach (CertificadoMatricula item in list)
                {
                    try
                    {
                        using (ExcelEngine excelEngine = new ExcelEngine())
                        {
                            //Initialize application
                            IApplication application = excelEngine.Excel;
                            OutPutPath = Path.Combine(rutaUploads + $"/CargueMatricula{DateTime.Now.ToString("yyyyMMdd")}", item.Documento.Trim() + ".pdf");
                            //Open existing workbook with data entered
                            var stream = System.IO.File.Open(fullFilePath, FileMode.Open, FileAccess.ReadWrite);
                            Stream fileStream = stream;
                            IWorkbook workbook = application.Workbooks.Open(fileStream, ExcelOpenType.Automatic);
                            IWorksheet worksheet = workbook.Worksheets[0];
                            worksheet.EnableSheetCalculations();
                            Stream nruta = System.IO.File.Create(EndPath);
                            //Add text data
                            try
                            {
                                worksheet.Range["B18"].Text = ConstruirTexto(item);
                                worksheet.Range["B28"].Text = ConstruirPie();
                                worksheet.Range["B24"].Text = GetMatricula(item);
                                worksheet.Range["B26"].Text = GetMensualidad(item);
                                workbook.SaveAs(nruta);
                                workbook.Close();
                                fileStream.Close();
                                excelEngine.Dispose();
                                nruta.Close();
                                SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                                var workbooks = ExcelFile.Load(EndPath);
                                workbooks.Worksheets.Remove(1);
                                var saveOptions = new PdfSaveOptions();
                                saveOptions.SelectionType = SelectionType.EntireFile;
                                workbooks.Save(OutPutPath, saveOptions);
                            }
                            catch (Exception ex)
                            {
                                workbook.Close();
                                fileStream.Close();
                                excelEngine.Dispose();
                                nruta.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return enviados;
        }
        private string GetMensualidad(CertificadoMatricula item)
        {
            if (item.Grado.Equals("TRANSICION"))
            {
                return String.Format("MENSUALIDAD: $90.000");
            }
            int grado = int.Parse(item.Grado);
            switch (grado)
            {
                case 1:
                    return "MENSUALIDAD: $90.000";
                case 2:
                    return "MENSUALIDAD: $90.000";
                case 3:
                    return "MENSUALIDAD: $90.000";
                case 4:
                    return "MENSUALIDAD: $90.000";
                case 5:
                    return "MENSUALIDAD: $90.000";
                case 6:
                    return "MENSUALIDAD: $100.000";
                case 7:
                    return "MENSUALIDAD: $120.000";
                case 8:
                    return "MENSUALIDAD: $120.000";
                case 9:
                    return "MENSUALIDAD: $130.000";
                case 10:
                    return "MENSUALIDAD: $130.000";
                case 11:
                    return "MENSUALIDAD: $140.000";
            }
            return string.Empty;
        }
        private string ConstruirPie()
        {
            DateTime fecha = DateTime.Now;
            string mes = GetMes();
            return string.Format("Se expide la presente constancia a petición de la parte interesada a los {0} días del mes de {1} de {2}", fecha.Day, mes, fecha.Year);
        }
        private string GetMatricula(CertificadoMatricula item)
        {
            if (item.Grado.Equals("TRANSICION"))
            {
                return String.Format("MATRICULA: $120.000");
            }
            int grado = int.Parse(item.Grado);
            switch (grado)
            {
                case 1:
                    return "MATRICULA: $120.000";
                case 2:
                    return "MATRICULA: $120.000";
                case 3:
                    return "MATRICULA: $120.000";
                case 4:
                    return "MATRICULA: $120.000";
                case 5:
                    return "MATRICULA: $120.000";
                case 6:
                    return "MATRICULA: $125.000";
                case 7:
                    return "MATRICULA: $145.000";
                case 8:
                    return "MATRICULA: $145.000";
                case 9:
                    return "MATRICULA: $145.000";
                case 10:
                    return "MATRICULA: $145.000";
                case 11:
                    return "MATRICULA: $145.000";
            }
            return string.Empty;
        }
        private string GetMes()
        {
            int Mes = DateTime.Now.Month;
            string NombreMes = string.Empty;
            switch (Mes)
            {
                case 1:
                    NombreMes = "Enero";
                    break;
                case 2:
                    NombreMes = "Febrero";
                    break;
                case 3:
                    NombreMes = "Marzo";
                    break;
                case 4:
                    NombreMes = "Abril";
                    break;
                case 5:
                    NombreMes = "Mayo";
                    break;
                case 6:
                    NombreMes = "Junio";
                    break;
                case 7:
                    NombreMes = "Julio";
                    break;
                case 8:
                    NombreMes = "Agosto";
                    break;
                case 9:
                    NombreMes = "Septiembre";
                    break;
                case 10:
                    NombreMes = "Octubre";
                    break;
                case 11:
                    NombreMes = "Noviembre";
                    break;
                case 12:
                    NombreMes = "Diciembre";
                    break;
            }
            return NombreMes;
        }
        private string ConstruirTexto(CertificadoMatricula item)
        {
            if (item.Grado.Equals("TRANSICION"))
            {
                return string.Format("Que el estudiante: {0} identificado(a) con {1} {2} se encuentra matriculado(a) en el grado {3} de Educación {4} correspondiente al año lectivo {5}.", item.Nombre, item.TipoDocumento, item.Documento, item.Grado, item.Educacion.Split('*')[1], DateTime.Now.Year.ToString());
            }
            else
            {
                return string.Format("Que el estudiante: {0} identificado(a) con {1} {2} se encuentra matriculado(a) en el grado {3}° {4} de Educación {5} correspondiente al año lectivo {6}.", item.Nombre, item.TipoDocumento, item.Documento, item.Grado, item.Educacion.Split('*')[0], item.Educacion.Split('*')[1], DateTime.Now.Year.ToString());
            }
            
        }
        #endregion
    }
}
