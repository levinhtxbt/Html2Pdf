using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Html2Pdf.Controllers
{
    [Produces("application/json")]
    [Route("html-to-pdf")]
    public class DefaultController : Controller
    {
        private readonly PdfGenerator _pdfGenerator;
        private readonly IConfiguration _configuration;

        public DefaultController(IHostingEnvironment host, IConfiguration configuration)
        {
            _pdfGenerator = new PdfGenerator(configuration, host);
            _configuration = configuration;
        }

        /// <summary>
        ///     Convert HTML to Pdf from Url
        /// </summary>
        /// <param name="url">from path</param>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult ExportPDF(string url)
        {
            try
            {
                ValidateSecretKey();

                var pdfPath = _pdfGenerator.ConvertHtmlToPdfFromUrl(url);

                if (!string.IsNullOrEmpty(pdfPath))
                    return Success(GetHostUrl() + pdfPath);

                return Failure();
            }
            catch (Exception ex)
            {
                return Failure(ex);
            }

        }

        /// <summary>
        ///     Convert HTML to Pdf from HTML Content
        /// </summary>
        /// <param name="html">from body</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> ExportPdf()
        {
            try
            {
                ValidateSecretKey();

                var html = await ReadContentBodyAsync();

                var pdfPath = _pdfGenerator.ConvertHtmlToPdfFromHtml(html);
                if (!string.IsNullOrEmpty(pdfPath))
                    return Success(GetHostUrl() + pdfPath);

                return Failure();
            }
            catch (Exception ex)
            {
                return Failure(ex);
            }

        }

        private OkObjectResult Success(string url) => Ok(new ResponseResource
        {
            Code = ResponseCode.Success,
            Data = url,
            Message = "Success"
        });

        private OkObjectResult Failure(Exception ex) => Ok(new ResponseResource
        {
            Code = ResponseCode.Failure,
            Message = ex.Message
        });

        private OkObjectResult Failure() => Ok(new ResponseResource
        {
            Code = ResponseCode.Failure,
            Message = "Cannot convert html to pdf"
        });

        private string GetHostUrl() => HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/";

        private void ValidateSecretKey()
        {
            var secretKey = _configuration.GetValue<string>("Secret-Key");
            var requestSecretKey = Request.Headers["Secret-Key"].ToString();

            if (string.IsNullOrEmpty(requestSecretKey))
                throw new Exception("Unauthorize");
            if (!secretKey.Equals(requestSecretKey))
                throw new Exception("Unauthorize");
        }

        private async Task<string> ReadContentBodyAsync()
        {
            var content = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                content = await reader.ReadToEndAsync();
            }

            return content;
        }

    }
}