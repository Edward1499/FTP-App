using FluentFTP;
using FTP_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net;

namespace FTP_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _appEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [Route("Home/OnPostUploadAsync")]
        public async Task<IActionResult> OnPostUploadAsync(IFormFile postedFile)
        {
            if (postedFile != null && postedFile.Length > 0) 
            {
                string name = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-" + postedFile.FileName;
                var filePath = _appEnvironment.WebRootPath + "/files/" + name;

                using (var stream = System.IO.File.Create(filePath))
                {
                    await postedFile.CopyToAsync(stream);
                }

                try
                {
                    var client = new FtpClient("demo.wftpserver.com", "demo", "demo");
                    client.Config.ValidateAnyCertificate = true;
                    client.AutoConnect();
                    client.UploadFile(@filePath, $"/upload/{name}");
                }
                catch(Exception ex) 
                {
                    return View("Index");
                }
            }
            return View("Index");
        }
    }
}