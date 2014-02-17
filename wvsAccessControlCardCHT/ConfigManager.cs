using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;

namespace wvsAccessControlCardCHT
{
    class ConfigManager
    {
        XElement _ConfigXml;

        /// <summary>
        /// 初始化解析
        /// </summary>
        public ConfigManager()
        {
            try
            {
                _ConfigXml = XElement.Load(Application.StartupPath + "\\Config.xml");
                if (_ConfigXml == null)
                    _ConfigXml = new XElement("Error");

                // DSA
                if (_ConfigXml.Element("DSA") != null)
                {
                    if (_ConfigXml.Element("DSA").Attribute("AccessPoint") != null)
                        DSA_AccessPoint = _ConfigXml.Element("DSA").Attribute("AccessPoint").Value;

                    if (_ConfigXml.Element("DSA").Attribute("ContractName") != null)
                        DSA_ContractName = _ConfigXml.Element("DSA").Attribute("ContractName").Value;

                    if (_ConfigXml.Element("DSA").Attribute("UserName") != null)
                        DSA_UserName = _ConfigXml.Element("DSA").Attribute("UserName").Value;

                    if (_ConfigXml.Element("DSA").Attribute("Password") != null)
                        DSA_Password = _ConfigXml.Element("DSA").Attribute("Password").Value;
                }

                if (_ConfigXml.Element("ClockAP") != null)
                {
                    if (_ConfigXml.Element("ClockAP").Attribute("Path") != null)
                        CAP_Path = _ConfigXml.Element("ClockAP").Attribute("Path").Value;

                    if (_ConfigXml.Element("ClockAP").Attribute("RunApp") != null)
                        CAP_RunApp = _ConfigXml.Element("ClockAP").Attribute("RunApp").Value;

                    CAP_WaitTime = 20;
                    if (_ConfigXml.Element("ClockAP").Attribute("WaitTime") != null)
                    {
                        int x;
                        if (int.TryParse(_ConfigXml.Element("ClockAP").Attribute("WaitTime").Value, out x))
                            CAP_WaitTime = x;
                    }
                }

                if (_ConfigXml.Element("ClockData") != null)
                {
                    if (_ConfigXml.Element("ClockData").Attribute("File") != null)
                        CAP_File = _ConfigXml.Element("ClockData").Attribute("File").Value;
                }

                if (_ConfigXml.Element("Backup") != null)
                {
                    if (_ConfigXml.Element("Backup").Attribute("Path") != null)
                        BackupPath = _ConfigXml.Element("Backup").Attribute("Path").Value;
                }

                //if (_ConfigXml.Element("CHT") != null)
                //{
                //    if (_ConfigXml.Element("CHT").Attribute("Account") != null)
                //        CHT_Account = _ConfigXml.Element("CHT").Attribute("Account").Value;

                //    if (_ConfigXml.Element("CHT").Attribute("Password") != null)
                //        CHT_Password = _ConfigXml.Element("CHT").Attribute("Password").Value;
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string DSA_AccessPoint { get; set; }
        public string DSA_ContractName { get; set; }
        public string DSA_UserName { get; set; }
        public string DSA_Password { get; set; }
        public string CAP_Path { get; set; }
        public string CAP_RunApp { get; set; }
        public string CAP_File { get; set; }
        public int CAP_WaitTime { get; set; }
        public string BackupPath { get; set; }
        //public string CHT_Account { get; set; }
        //public string CHT_Password { get; set; }

        /// <summary>
        /// 取得所有XML
        /// </summary>
        /// <returns></returns>
        public XElement GetAll()
        {
            return _ConfigXml;
        }        
    }
}
