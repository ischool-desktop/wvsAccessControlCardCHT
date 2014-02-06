using FISCA.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace wvsAccessControlCardCHT
{
    class MsgManager
    {
        QueryHelper _Q;

        public MsgManager(QueryHelper _Q)
        {
            this._Q = _Q;
        }

        public MsgObj Start()
        {
            string sqlCMD = "SELECT * FROM $cht_access_control_card.setting";
            DataTable dt = _Q.Select(sqlCMD);

            foreach(DataRow row in dt.Rows)
            {
                string arrive_school_sms = row["arrive_school_sms"].ToString();
                string leave_school_sms = row["leave_school_sms"].ToString();
                string error_phone = row["error_phone"].ToString();
                bool enable_arrive_school_sms = row["enable_arrive_school_sms"].ToString() == "true" ? true : false;
                bool enable_leave_school_sms = row["enable_leave_school_sms"].ToString() == "true" ? true : false;
                bool enable_error_sms = row["enable_error_sms"].ToString() == "true" ? true : false;

                MsgObj obj = new MsgObj(arrive_school_sms, leave_school_sms, error_phone, enable_arrive_school_sms, enable_leave_school_sms, enable_error_sms);

                return obj; 
            }

            return null;
        }
    }
}
