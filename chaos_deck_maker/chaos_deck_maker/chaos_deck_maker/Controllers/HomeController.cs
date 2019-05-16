using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Entity;
using Newtonsoft.Json;
using System.Text;

namespace chaos_deck_maker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Search(FormCollection form)
        {
            var keyword = form["keyword"];
            var page = form["page"];

            if (String.IsNullOrEmpty(keyword) || String.IsNullOrEmpty(page))
            {
                return Json(new { Success = false });
            }

            var entityCard = new Card();
            var cardPool = entityCard.Search(keyword, Convert.ToInt32(page));
            if(cardPool.Count() > 0)
            {
                return Json(new { Success = true, CardPool = JsonConvert.SerializeObject(cardPool) });
            }
            else
            {
                return Json(new { Success = false });
            }
        }
        public JsonResult Export(FormCollection form)
        {
            var decks = form["decks"];

            if (String.IsNullOrEmpty(decks))
            {
                return Json(new { Success = false });
            }

            var path = $"/Upload/Decks/{DateTime.Now.ToString("MM-dd-HHmmss")}.txt";

            FileStream fs = new FileStream(Server.MapPath(path), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            sw.Write(decks);

            sw.Close();
            fs.Close();

            return Json(new { Success = true , Path = path });
        }
    }
}