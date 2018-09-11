using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SOOS_Auction.AuctionGoogleDrive;
using SOOS_Auction.Models;
using SOOS_Auction.Models.ImagesRequests;

namespace SOOS_Auction.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ApplicationDBContext contt = new ApplicationDBContext();
            
            //ApiFunctions.GetFiles();
            return View();
        }

        public ActionResult About()
        {
            AuctionContext auctionContext = new AuctionContext();
            auctionContext.Bids.ToList();
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //
        [HttpPost]
        public JsonResult ServerUpload()
        {
            string __filepath = Server.MapPath("~/uploads");
            int __maxSize = 2 * 1024 * 1024;    // максимальный размер файла 2 Мб
                                                // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result
            {
                Files = new List<string>()
            };
            

            if (Request.Files.Count > 0)
            {
                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[f];

                    // Выполнить проверки на допустимый размер файла и формат
                    if (file.ContentLength > __maxSize)
                    {
                        result.Error = "Размер файла не должен превышать 2 Мб";
                        break;
                    }
                    else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                    {
                        result.Error = "Недопустимый формат файла";
                        break;
                    }
                    // Сохранить файл и вернуть URL
                    if (Directory.Exists(__filepath))
                    {
                        Guid guid = Guid.NewGuid();
                        file.SaveAs($@"{__filepath}\{guid}.{file.FileName}");
                        string fileId = FileApiMethods.Upload("lul", $@"{__filepath}\{guid}.{file.FileName}", file.ContentType);
                        FileApiMethods.MakeFilePublic(fileId);
                        result.Files.Add("https://drive.google.com/uc?id=" + fileId);
                        System.IO.File.Delete($@"{__filepath}\{guid}.{file.FileName}");
                    }
                }
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Upload()
        {
            string __filepath = Server.MapPath("~/uploads");
            string CurrentUserId = User.Identity.GetUserId();
            int __maxSize = 2 * 1024 * 1024;    // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result
            {
                Files = new List<string>()
            };

            if (Request.Files.Count > 0)
            {
                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[f];

                    // Выполнить проверки на допустимый размер файла и формат
                    if (file.ContentLength > __maxSize)
                    {
                        result.Error = "Размер файла не должен превышать 2 Мб";
                        break;
                    }
                    else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                    {
                        result.Error = "Недопустимый формат файла";
                        break;
                    }

                    // Сохранить файл и вернуть URL
                    if (Directory.Exists(__filepath))
                    {
                        Guid guid = Guid.NewGuid();
                        file.SaveAs($@"{__filepath}\{guid}.{file.FileName}");
                        result.Files.Add($"/uploads/{guid}.{file.FileName}");
                    }
                }
            }

            return Json(result);
        }
    }
}
