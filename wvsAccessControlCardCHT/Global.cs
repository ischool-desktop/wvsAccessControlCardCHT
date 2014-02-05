using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAClient;
using System.Data;
using System.Xml.Linq;

namespace wvsAccessControlCardCHT
{
    class Global
    {
        /// <summary>
        /// 設定檔
        /// </summary>
        public static ConfigManager _Config;

        /// <summary>
        /// 取得 DSA 連線
        /// </summary>
        /// <returns></returns>
        public static Connection GetDSAConnection()
        {
            Connection cn = new Connection();
            cn.EnableSession = false;
            cn.Connect(Global._Config.DSA_AccessPoint, Global._Config.DSA_ContractName, Global._Config.DSA_UserName, Global._Config.DSA_Password);
            return cn;
        }

    }
}
