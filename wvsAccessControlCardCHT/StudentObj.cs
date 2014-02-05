using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wvsAccessControlCardCHT
{
    public class StudentObj
    {
        private string cardNo, id, name, cellPhone;

        public StudentObj(string cardNo, string id, string name, string cellPhone)
        {
            this.cardNo = cardNo;
            this.id = id;
            this.name = name;
            this.cellPhone = cellPhone;
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string CardNo
        {
            get { return cardNo; }
            set { cardNo = value; }
        }

        public string CellPhone
        {
            get { return cellPhone; }
            set { cellPhone = value; }
        }
    }
}
