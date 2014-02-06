using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Data;
using FISCA.Data;

namespace wvsAccessControlCardCHT
{
    class SMSSender
    {
        bool _Enable_arrive, _Enable_leave, _Enable_error;
        UpdateHelper _U;
        QueryHelper _Q;
        Dictionary<string, string> ReturnCodeDic;

        public SMSSender(QueryHelper _Q)
        {
            this._Q = _Q;
            #region 中華電信回傳字典
            ReturnCodeDic = new Dictionary<string, string>();
            ReturnCodeDic.Add("-1", "系統或是資料庫故障。");
            ReturnCodeDic.Add("0", "訊息已成功發送至接收端。");
            ReturnCodeDic.Add("1", "訊息傳送中。");
            ReturnCodeDic.Add("2", "系統無法找到您要找的訊息。請檢查你的 toaddr 和messageid 是否正確。");
            ReturnCodeDic.Add("3", "訊息無法成功送達手機。");
            ReturnCodeDic.Add("4", "系統或是資料庫故障。");
            ReturnCodeDic.Add("5", "訊息狀態不明。此筆訊息已被刪除。");
            ReturnCodeDic.Add("8", "接收端 SIM 已滿，造成訊息傳送失敗。");
            ReturnCodeDic.Add("9", "錯誤的接收端號碼，可能是空號。");
            ReturnCodeDic.Add("11", "號碼格式錯誤。");
            ReturnCodeDic.Add("12", "收訊手機已設定拒收簡訊。");
            ReturnCodeDic.Add("13", "手機錯誤。");
            ReturnCodeDic.Add("16", "系統無法執行msisdn<->subno，請稍候再試。");
            ReturnCodeDic.Add("17", "系統無法找出對應此 subno之電話號碼，請查明 subno是否正確。");
            ReturnCodeDic.Add("18", "請檢查受訊方號碼格式是否正確。");
            ReturnCodeDic.Add("21", "請檢查 Message id 格式是否正確。");
            ReturnCodeDic.Add("23", "你的登入 IP 未在系統註冊。");
            ReturnCodeDic.Add("24", "帳號已停用。");
            ReturnCodeDic.Add("31", "訊息尚未傳送到 SMSC 。");
            ReturnCodeDic.Add("32", "訊息無法傳送到簡訊中心。");
            ReturnCodeDic.Add("33", "訊息無法傳送到簡訊中心(訊務繁忙)。");
            ReturnCodeDic.Add("48", "受訊客戶要求拒收加值簡訊，請不要再重送。");
            ReturnCodeDic.Add("55", "http (port 8008) 連線不允許使用 GET 方法，請改用 POST 或改為 https(port 4443) 連線。");
            #endregion
        }

        public void Start(Dictionary<string, CardData> cardDic, MsgObj msgObj)
        {
            _U = new UpdateHelper(Global.GetDSAConnection());
            _Enable_arrive = msgObj.Enable_arrive;
            _Enable_leave = msgObj.Enable_leave;
            _Enable_error = msgObj.Enable_error;

            if (cardDic != null)
            {
                foreach (KeyValuePair<string, CardData> card in cardDic)
                {
                    //有學生資料才傳送
                    if (card.Value.StudentInfo != null)
                    {
                        //設定傳送訊息(姓名)-到校且有啟動時
                        if (card.Value.UseType == "01" && _Enable_arrive)
                        {
                            card.Value.SendMsg = msgObj.Arrive_msg.Replace("{姓名}", card.Value.StudentInfo.Name);
                        }
                        //設定傳送訊息(姓名)-離校且有啟動時
                        if (card.Value.UseType == "02" && _Enable_leave)
                        {
                            card.Value.SendMsg = msgObj.Leave_msg.Replace("{姓名}", card.Value.StudentInfo.Name);
                        }

                        //SendMsg有值才進行
                        if (!string.IsNullOrEmpty(card.Value.SendMsg))
                        {
                            //設定傳送訊息(到離時間)
                            card.Value.SendMsg = card.Value.SendMsg.Replace("{時間}", card.Value.Date + " " + card.Value.Time);

                            //傳送
                            string[] result = Submit(card.Value);

                            //紀錄
                            WriteLog(card.Value, result);
                        }
                    }
                    else
                    {//查無學生相關資料時
                        //有啟動到校提醒需寫Log
                        if (card.Value.UseType == "01" && _Enable_arrive)
                        {
                            WriteLog(card.Value, null);
                        }
                        //有啟動離校提醒需寫Log
                        if (card.Value.UseType == "02" && _Enable_leave)
                        {
                            WriteLog(card.Value, null);
                        }
                    }
                }
            }
            else
            {
                //異常提醒功能有開啟
                if (_Enable_error)
                    Alert(msgObj);
            }
        }

        private string[] Submit(CardData card)
        {
            // 測試帳號
            string account = "14525";
            // 測試密碼
            string password = "14525";

            //儲存發送日期
            card.SendDateTime = SelectTime();

            // 訊息編碼 
            string msg = HttpUtility.UrlEncode(card.SendMsg, System.Text.Encoding.Default);
            //string msg = HttpUtility.UrlEncode(txtMsg.Text, System.Text.Encoding.Unicode);

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("http://imsp.emome.net:8008/imsp/sms/servlet/SubmitSM?account=");
                // 測試組資料
                /*http://imsp.emome.net:8008/imsp/sms/servlet/SubmitSM?account=14525&password=14525&from_addr_type=0&from_addr=&to_addr_type=0&to_addr=0937347036&msg_expire_time=0&msg_type=0&msg=%e6%b8%ac%e8%a9%a6
                //https://imsp.emome.net:4443/imsp/sms/servlet/SubmitSM?account=14525&password=14525&from_addr_type=0&from_addr=&to_addr_type=0&to_addr=0937347036&msg_expire_time=0&msg_type=0&msg=%aea%aex
                */
                sb.Append(account);
                sb.Append("&password=" + password);
                sb.Append("&from_addr_type=" + "0");
                sb.Append("&from_addr=" + "");
                sb.Append("&to_addr_type=" + "0");
                sb.Append("&to_addr=" + card.StudentInfo.CellPhone);
                sb.Append("&msg_expire_time=" + "0");
                sb.Append("&msg_type=" + "0");
                sb.Append("&msg=" + msg);

                WebClient client = new WebClient();

                string response = "";
                //用using來自動關閉資源
                using (Stream data = client.OpenRead(sb.ToString()))
                {
                    using (StreamReader reader = new StreamReader(data))
                    {
                        //讀取每一行
                        while ((response = reader.ReadLine()) != null)
                        {
                            //直到<body>再讀取下一行
                            if (response.StartsWith("<body>"))
                            {
                                //取得response後跳離
                                response = reader.ReadLine();
                                break;
                            }
                        }
                    }
                }

                //刪去結尾<br>標記
                response = response.Replace("<br>", "");

                //response拆解
                string[] result = response.Split('|');

                //回傳結果
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("簡訊發送異常,發送行為將停止...\r\n原因是:" + ex.Message);
                return null;
            }
        }


        private void WriteLog(CardData obj, string[] result)
        {
            string cht_msg_id, cht_status,cht_message, send_message;
            string cht_chk_date = "";

            if(result != null)
            {
                cht_msg_id = result[2];
                cht_status = result[1];
                cht_chk_date = obj.SendDateTime;
                cht_message = ReturnCodeDic.ContainsKey(cht_status) ? ReturnCodeDic[cht_status] : result[3];
                send_message = obj.SendMsg;
            }
            else
            {
                cht_msg_id = "";
                cht_status = "";
                cht_message = "";
                send_message = "";
            }

            string sqlCMD = "INSERT INTO $cht_access_control_card.history(card_no,oclock_name,use_time,use_type,ref_student_id,cell_phone,send_date,send_message,cht_msg_id,cht_status,cht_message,cht_chk_date)";

            sqlCMD += string.Format(" VALUES('{0}','{1}','{2}','{3}',{4},'{5}',{6},'{7}','{8}','{9}','{10}',{11})",
                obj.CardNo,
                obj.ClockNo,
                ParseDateTime(obj.Date, obj.Time).ToString("yyyy/MM/dd HH:mm:ss"),
                obj.UseType,
                obj.StudentInfo == null ? "null" : obj.StudentInfo.Id,
                obj.StudentInfo == null ? "" : obj.StudentInfo.CellPhone,
                obj.SendDateTime == null ? "null" : "'" + obj.SendDateTime + "'",
                send_message,
                cht_msg_id,
                cht_status,
                cht_message,
                cht_chk_date == "" ? "null" : "'" + cht_chk_date + "'"
                );

            _U.Execute(sqlCMD);
        }

        private string SelectTime()
        {
            //取得時間
            DataTable dtable = _Q.Select("SELECT NOW()");
            
            //Parse資料
            DateTime dt = DateTime.Now;
            DateTime.TryParse("" + dtable.Rows[0][0], out dt);
            
            //最後時間
            string ComputerSendTime = dt.ToString("yyyy/MM/dd HH:mm:ss");

            return ComputerSendTime;
        }

        private DateTime ParseDateTime(string date,string time)
        {
            IFormatProvider culture = new System.Globalization.CultureInfo("zh-TW", true);
            DateTime datetime = DateTime.ParseExact(date+time, "yyyyMMddHHmm", culture);

            return datetime;
        }

        //錯誤提醒簡訊
        private void Alert(MsgObj obj)
        {
            // 測試帳號
            string account = "14525";
            // 測試密碼
            string password = "14525";

            // 訊息編碼
            string msg = HttpUtility.UrlEncode("門禁刷卡機卡機異常,請確認", System.Text.Encoding.Default);
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("http://imsp.emome.net:8008/imsp/sms/servlet/SubmitSM?account=");
                // 測試組資料
                /*http://imsp.emome.net:8008/imsp/sms/servlet/SubmitSM?account=14525&password=14525&from_addr_type=0&from_addr=&to_addr_type=0&to_addr=0937347036&msg_expire_time=0&msg_type=0&msg=%e6%b8%ac%e8%a9%a6
                //https://imsp.emome.net:4443/imsp/sms/servlet/SubmitSM?account=14525&password=14525&from_addr_type=0&from_addr=&to_addr_type=0&to_addr=0937347036&msg_expire_time=0&msg_type=0&msg=%aea%aex
                */
                sb.Append(account);
                sb.Append("&password=" + password);
                sb.Append("&from_addr_type=" + "0");
                sb.Append("&from_addr=" + "");
                sb.Append("&to_addr_type=" + "0");
                sb.Append("&to_addr=" + obj.Error_phone);
                sb.Append("&msg_expire_time=" + "0");
                sb.Append("&msg_type=" + "0");
                sb.Append("&msg=" + msg);

                WebClient client = new WebClient();

                //用using來自動關閉資源
                using (Stream data = client.OpenRead(sb.ToString()))
                {
                    //do notiing...
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("簡訊發送異常,發送行為將停止...\r\n原因是:" + ex.Message);
            }
        }
    }
}

