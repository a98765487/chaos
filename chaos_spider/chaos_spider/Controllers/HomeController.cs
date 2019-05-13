using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Utility;
using Entity;
using System.IO;

namespace chaos_spider.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //chaos spider start

            //所有系列的陣列
            string[] allSeriesAry = new string[] { "promo" };


            var imgPattern = "<div class=\"card_detail_box[\\s\\S]*?(?:img src=\"([\\s\\S]*?)\")[\\s\\S]*?<\\/div>";
            var cardInfoPattern = "<div class=\"information_box\">[\\s\\S]*?<th>([\\s\\S]*?)<\\/th>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<td>([\\s\\S]*?)<\\/td>[\\s\\S]*?<\\/div>";
            var domain = ConfigurationManager.AppSettings["Domain"].ToString();
            Card entityCard = new Card();

            Regex regex = null;
            Match match = null;

            foreach (var item in allSeriesAry)
            {
                for (var i = 10028; i < 10200; i++)
                {
                    var html = CommHelper.GetHttpPageSource("https://yuyu-tei.jp/game_chaos/carddetail/cardpreview.php?VER=" + item + "&CID=" + i + "&MODE=sell");

                    if (String.IsNullOrEmpty(html))
                    {
                        break;
                    }

                    Card.Info info = new Card.Info();

                    regex = new Regex(imgPattern, RegexOptions.IgnoreCase);
                    match = regex.Match(html);
                    if (match.Captures.Count > 0)
                    {
                        var imgUri = new Uri(domain + match.Groups[1].Value);

                        if(imgUri.Segments.Count() < 6)
                        {
                            continue;
                        }
                        var folder = Server.MapPath("/Images/" + imgUri.Segments[4].TrimEnd('/'));

                        info.IMG_FOLDER = imgUri.Segments[4].TrimEnd('/');
                        info.IMG_NAME = imgUri.Segments[5].TrimEnd('/');

                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        CommHelper.GetImageSource(imgUri.ToString(), folder, imgUri.Segments[5].TrimEnd('/'));

                    }
                    else
                    {
                        continue;
                    }

                    regex = new Regex(cardInfoPattern, RegexOptions.IgnoreCase);
                    match = regex.Match(html);
                    if (match.Captures.Count > 0)
                    {
                        info.CARD_NUMBER = match.Groups[1].Value.Trim();
                        //卡片名稱還要再分割
                        var strAry = match.Groups[2].Value.Trim().Split('\t');
                        var nameAry = strAry[0].Split(' ');
                        info.RARITY = nameAry[0];
                        info.NAME = String.Join("", nameAry.Skip(1));
                        info.ATTR = match.Groups[3].Value;
                        info.ATK = match.Groups[4].Value;
                        info.DEF = match.Groups[5].Value;
                        info.ATK_COR = match.Groups[6].Value;
                        info.DEF_COR = match.Groups[7].Value;
                        info.TYPE = match.Groups[8].Value;
                        info.SERIES = match.Groups[9].Value;
                        info.EFFECT = match.Groups[10].Value.Trim().Replace("<br>", "").Replace("<p>", "").Replace("</p>", "");
                        info.LINE = match.Groups[11].Value;

                    }
                    else
                    {
                        continue;
                    }

                    if(entityCard.Select(info.CARD_NUMBER).Count() > 0)
                    {
                        continue;
                    }
                    else
                    {
                        entityCard.Add(info.CARD_NUMBER, info.TYPE, info.ATTR, info.ATK, info.DEF, info.ATK_COR, info.DEF_COR, info.SERIES, info.EFFECT, info.LINE, info.IMG_FOLDER, info.IMG_NAME, info.NAME, info.RARITY);
                    }
                }
            }

            

            return View();
        }
    }
}