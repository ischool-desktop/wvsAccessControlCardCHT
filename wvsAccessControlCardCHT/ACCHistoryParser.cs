using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data;
using FISCA.Data;

namespace wvsAccessControlCardCHT
{
    /// <summary>
    /// 解析讀卡資料
    /// </summary>
    public class ACCHistoryParser
    {
        Dictionary<string, CardData> _CardDic;
        Dictionary<string,StudentObj> _StudentDic;
        QueryHelper _Q;
        string sourceFileName = @Global._Config.CAP_File;
        string descFileName = "";

        public ACCHistoryParser(QueryHelper _Q)
        {
            this._Q = _Q;
        }

        /// <summary>
        /// 取得檔案內紀錄
        /// </summary>
        public Dictionary<string, CardData> getFileRecord()
        {
           bool pass = true;
           Dictionary<string,CardData> retVal = new Dictionary<string,CardData>();

            try
            {
                if (File.Exists(sourceFileName))
                {
                    // 讀取讀卡資料檔
                    StreamReader sr = new StreamReader(sourceFileName);

                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        CardData card = new CardData(line);
                        //判斷是否解析成功
                        if(card.GetPass())
                        {
                            string key = card.CardNo + "_" + card.Date + "_" + card.UseType;
                            if (!retVal.ContainsKey(key))
                            {
                                retVal.Add(key, card);
                            }
                        }
                        else
                        {
                            pass &= false;
                        }
                    }

                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getFileRecord():" + ex.Message);
            }

            if (pass)
                return retVal;
            else
                return null;
        }

        /// <summary>
        /// 取得詳細資訊
        /// </summary>
        public void GetInfo()
        {
            // 取得讀卡資料
            _CardDic = getFileRecord();

            if (_CardDic == null) return;

            List<string> cardNoList = new List<string>();

            foreach (KeyValuePair<string, CardData> card in _CardDic)
            {
                if (!cardNoList.Contains(card.Value.CardNo))
                {
                    //組SQL用的卡號清單
                    cardNoList.Add(card.Value.CardNo);
                }
            }

            //取得學生資料-組SQL
            string str = string.Join("','", cardNoList);
            str = "'" + str + "'";
            string sqlCMD = "SELECT card.*,student.name FROM $cht_access_control_card.student_cardno AS card";
            sqlCMD += " JOIN student ON card.ref_student_id=student.id";
            sqlCMD += " WHERE card_no in (" + str + ")";

            //取得學生資料-Query
            DataTable dt = _Q.Select(sqlCMD);

            //建立字典
            _StudentDic = new Dictionary<string, StudentObj>();
            foreach(DataRow row in dt.Rows)
            {
                string cardNo = row["card_no"].ToString();
                string studentID = row["ref_student_id"].ToString();
                string name = row["name"].ToString();
                string cellPhone = row["cell_phone"].ToString();

                //儲存學生資訊
                if (!_StudentDic.ContainsKey(cardNo))
                    _StudentDic.Add(cardNo, new StudentObj(cardNo, studentID, name, cellPhone));
            }

            //設定學生對應資料
            foreach(KeyValuePair<string,CardData> card in _CardDic)
            {
                if (_StudentDic.ContainsKey(card.Value.CardNo))
                card.Value.StudentInfo = _StudentDic[card.Value.CardNo];
            }

            //備份刷卡紀錄
            try
            {
                if (File.Exists(sourceFileName))
                {
                    // 讀完後備份到另一個地方
                    descFileName = @Global._Config.BackupPath + "\\card_no_history" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    File.Move(sourceFileName, descFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("刷卡資料檔搬移失敗:" + ex.Message);
            }
        }

        /// <summary>
        /// 啟動
        /// </summary>
        public Dictionary<string,CardData> Start()
        {
            try
            {
                GetInfo();
                return _CardDic;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ACCHistoryParser:" + ex.Message);
                return null;
            }
        }
    }
}
