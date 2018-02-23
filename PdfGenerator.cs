using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Html2Pdf
{
    public class PdfGenerator
    {
        private readonly string _wkhtmltopdfDir = string.Empty;
        private readonly string _pdfDir = string.Empty;
        private readonly IHostingEnvironment _host;

        public PdfGenerator(IConfiguration configuration, IHostingEnvironment host)
        {
            _wkhtmltopdfDir = configuration.GetValue<string>("WkhtmltopdfDir");
            _pdfDir = configuration.GetValue<string>("PdfDir");
            _host = host;
        }

        public string ConvertHtmlToPdfFromUrl(string url)
        {
            ValidateWKHtmltToPdf();

            var result = ExecuteProcess(url);

            if (!string.IsNullOrEmpty(result))
                return _pdfDir + "/" + result;

            return string.Empty;
        }

        public string ConvertHtmlToPdfFromHtml(string html)
        {
            ValidateWKHtmltToPdf();

            var htmlPath = SaveHtmlToFile(html);

            try
            {
                var result = ExecuteProcess(htmlPath);

                if (!string.IsNullOrEmpty(result))
                    return _pdfDir + "/" + result;

                return string.Empty;
            }
            finally
            {
                File.Delete(htmlPath);
            }
        }

        private string SaveHtmlToFile(string html)
        {
            var fileName = "temp_" + Guid.NewGuid().ToString() + ".html";
            var tempPath = _host.ContentRootPath + "\\Temp\\";
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            var filePath = tempPath + fileName;

            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.WriteLine(html);
                    w.Close();
                }

                fs.Close();
            }

            return tempPath + fileName;
        }

        private string GetPdfWorkingDirectory()
        {
            var workingDir = Path.Combine(_host.WebRootPath, _pdfDir);
            if (!Directory.Exists(workingDir))
                Directory.CreateDirectory(workingDir);
            return workingDir;
        }

        private void ValidateWKHtmltToPdf()
        {
            if (string.IsNullOrEmpty(_wkhtmltopdfDir))
                throw new Exception("Invalid wkhtmltopdfDir");
        }

        private string ExecuteProcess(string path)
        {
            var fileName = Guid.NewGuid().ToString() + ".pdf";
            var p = new Process();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = _wkhtmltopdfDir;
            p.StartInfo.WorkingDirectory = GetPdfWorkingDirectory();

            string switches = "";
            switches += "--print-media-type ";
            switches += "--dpi 96 ";
            switches += "--zoom 1.3 ";
            switches += "--viewport-size 720x1280 ";
            switches += "--orientation Portrait ";
            switches += "--page-size A4 ";
            switches += "--enable-javascript --javascript-delay 200 ";
            switches += "--margin-top 0mm --margin-bottom 0mm --margin-right 0mm --margin-left 0mm ";
            p.StartInfo.Arguments = switches + " " + path + " " + fileName;

            try
            {
                p.Start();
                p.WaitForExit(60000);

                int returnCode = p.ExitCode;
                return returnCode == 0 ? fileName : string.Empty;
            }
            finally
            {
                p.Close();
            }

        }
    }
}