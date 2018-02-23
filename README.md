# Html2Pdf
This is a sample C# wrapper utility around wkhtmltopdf console tool. You can use it to easily convert HTML report to PDF

## Persistence
- ASP.Net Core 2.0 SDK
- [wkhtmltopdf](https://wkhtmltopdf.org/downloads.html)

## Installation
- Step 1: Install wkhtmltopdf to local machine/ server
- Step 2: Update wkhtmltopdf directory in appsettings.json  `"WkhtmltopdfDir": "C:\\Developement\\wkhtmltopdf\\bin\\wkhtmltopdf.exe",`
- Step 3: In root directory, Excute these command line `dotnet restore` , `dotnet build`, `dotnet run`.
- Step 4: Import **HtmlToPdf.postman_collection.json** to postman for testing.
