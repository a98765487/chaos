using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace chaos_card_maker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult UploadOriginImg(HttpPostedFileBase file)
        {
            string oriImgSrc = String.Empty;
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/upload"), fileName);
                file.SaveAs(path);
                if (System.IO.File.Exists(path))
                {
                    oriImgSrc = "/upload/" + fileName;
                }
            }
            else
            {
                return Json(new { Success = false, Messege = "找不到圖片" });
            }
            


            return Json(new { Success = true , oriImgSrc , Messege="上傳圖片成功" });
        }
        public JsonResult CropImg(FormCollection form)
        {
            var OriginImgSrc = form["Src"];
            var X = Convert.ToInt32(form["X"]);
            var Y = Convert.ToInt32(form["Y"]);
            var Width = Convert.ToInt32(form["Width"]);
            var Height = Convert.ToInt32(form["Height"]);
            var NewFileName = "/upload/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
            var TargetImgPath = Path.Combine(Server.MapPath("~" + NewFileName));
            
            if (!String.IsNullOrEmpty(OriginImgSrc))
            {
                OriginImgSrc = Path.Combine(Server.MapPath("~" + OriginImgSrc));
                if (System.IO.File.Exists(OriginImgSrc))
                {
                    //原始圖片
                    System.Drawing.Image sourceImage = Image.FromFile(OriginImgSrc);

                    //裁剪的區域
                    Rectangle fromR = new Rectangle(X,Y,Width,Height);
                    //裁剪出來的圖要放在畫布的哪個位置
                    Rectangle toR = new Rectangle(0, 0,Width, Height);
                    //要當畫布的Bitmap物件
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(Width, Height);
                    //產生畫布
                    System.Drawing.Graphics g = Graphics.FromImage(bitmap);
                    //清空畫布，背景白色
                    g.Clear(Color.White);

                    //以像素做為測量單位
                    GraphicsUnit units = GraphicsUnit.Pixel;
                    //剪裁
                    g.DrawImage(sourceImage, toR, fromR, units);



                    //裁剪完成，存檔
                    bitmap.Save(TargetImgPath, ImageFormat.Jpeg);
                    //釋放資源
                    g.Dispose();
                    bitmap.Dispose();
                    sourceImage.Dispose();
                }
            }

            return Json(new { Success = true, newImageSrc = NewFileName, Messege = "裁切圖片成功" });
        }
    }
}