using FISCA.Data;
using FISCA.DSAUtil;
using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wvsAccessControlCardCHT
{
    class Program
    {
        public static void Main()
        {
            Dictionary<string, CardData> _CardDic;
            MsgObj msgObj;
            
            // 讀取設定檔
            Global._Config = new ConfigManager();
            QueryHelper _Q = new QueryHelper(Global.GetDSAConnection());

            // 執行讀卡鐘            
            ACCHistoryCollector _ACCHistoryCollector = new ACCHistoryCollector();
            _ACCHistoryCollector.Start();

            // 執行讀取資料、比對、解析            
            ACCHistoryParser _ACCHistoryParser = new ACCHistoryParser(_Q);
            _CardDic = _ACCHistoryParser.Start();

            //取得訊息內容&設定
            MsgManager MM = new MsgManager(_Q);
            msgObj = MM.Start();

            //SMS傳送
            SMSSender smss = new SMSSender(_Q);
            smss.Start(_CardDic, msgObj);
        }
    }
}
