using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniProject.Models;
using System.Diagnostics;
using System.Linq;
using OfficeOpenXml;

namespace MiniProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload()
        {
            var file = Request.Form.Files.First();
            var fileName = new UploadedFile(file).Save();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            return Json(new DataTableWithHeaders(fileName).AsJson());
        }

        [HttpGet]
        public ContentResult Export(string fileName, string format, string sortedBy, string delimiter)
        {
            string result;

            var dataTable = new DataTableWithHeaders(fileName);
            if (!string.IsNullOrEmpty(sortedBy))
            {
                dataTable.SortTable(sortedBy);
            }

            switch (format)
            {
                case "xml":
                    result = dataTable.AsXml();
                    break;
                case "json":
                    result = dataTable.AsJson();
                    break;
                case "delimiter":
                    result = dataTable.AsDelimited(delimiter);
                    break;
                default:
                    result = dataTable.AsJson();
                    break;

            }

            return Content(result);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
