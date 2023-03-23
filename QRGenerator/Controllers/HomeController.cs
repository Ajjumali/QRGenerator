using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRGenerator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace QRGenerator.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        public IActionResult Index(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return View(BitmapToBytes(qrCodeImage));
        }
        public IActionResult Index()
        {
            return View();
        }
        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public IActionResult GenerateFile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateFile(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);
            qrCodeData.SaveRawData("wwwroot/qrr/file-" + fileGuid + ".qrr", QRCodeData.Compression.Uncompressed);

            QRCodeData qrCodeData1 = new QRCodeData("wwwroot/qrr/file-" + fileGuid + ".qrr", QRCodeData.Compression.Uncompressed);
            QRCode qrCode = new QRCode(qrCodeData1);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            
            return View(BitmapToBytes(qrCodeImage));
        }
        public IActionResult ViewFile()
        {
            List<KeyValuePair<string, Byte[]>> fileData = new List<KeyValuePair<string, byte[]>>();
            KeyValuePair<string, Byte[]> data;

            string[] files = Directory.GetFiles("wwwroot/qrr");
            foreach (string file in files)
            {
                QRCodeData qrCodeData = new QRCodeData(file, QRCodeData.Compression.Uncompressed);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                Byte[] byteData = BitmapToBytes(qrCodeImage);
                data = new KeyValuePair<string, Byte[]>(Path.GetFileName(file), byteData);
                fileData.Add(data);
            }

            return View(fileData);
        }
        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
