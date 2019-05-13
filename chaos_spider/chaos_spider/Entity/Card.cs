using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Card
    {
        private string ConnStr { get; set; }
        public Card()
        {
            this.ConnStr = ConfigurationManager.AppSettings["ConnInfo"].ToString();
        }
        //表格資料
        public class Info
        {
            public string SID { get; set; }
            public string CARD_NUMBER { get; set; }
            public string TYPE { get; set; }
            public string ATTR { get; set; }
            public string ATK { get; set; }
            public string DEF { get; set; }
            public string ATK_COR { get; set; }
            public string DEF_COR { get; set; }
            public string SERIES { get; set; }
            public string EFFECT { get; set; }
            public string LINE { get; set; }
            public string IMG_FOLDER { get; set; }
            public string IMG_NAME { get; set; }
            public string NAME { get; set; }
            public string RARITY { get; set; }
        }
        public List<Info> Select(string cardNumber)
        {

            List<Info> results = new List<Info>();
            try
            {
                using (SqlConnection conn = new SqlConnection(this.ConnStr))
                {
                    results = conn.Query<Info>("usp_CARDSelect"
                                , new { CARD_NUM = cardNumber }
                                , commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                return results;
            }
            return results;
        }
        public bool Add(string cardNumber, string type, string attr, string atk, string def, string atkCor, string defCor, string series,string effect, string line, string imgFolder, string imgName, string name, string rarity)
        {
            try
            {
                using (var conn = new SqlConnection(this.ConnStr))
                {
                    var sid = Guid.NewGuid().ToString("N").Substring(0, 20);
                    string procName = "usp_CARDInsert";
                    var data = new { SID = sid, CARD_NUMBER = cardNumber, TYPE = type, ATTR = attr, ATK = atk, DEF = def, ATK_COR = atkCor, DEF_COR = defCor, SERIES = series, EFFECT = effect, LINE = line, IMG_FOLDER = imgFolder, IMG_NAME = imgName, NAME = name, RARITY = rarity };
                    conn.Execute(procName, data, commandType: CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
