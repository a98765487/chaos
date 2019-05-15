using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Entity;
using Newtonsoft.Json;

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
    }
}