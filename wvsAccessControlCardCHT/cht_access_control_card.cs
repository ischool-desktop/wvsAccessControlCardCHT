using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wvsAccessControlCardCHT
{
    [FISCA.UDT.TableName("cht_access_control_card.student_cardno")]
    class cht_access_control_card : FISCA.UDT.ActiveRecord
    {
        [FISCA.UDT.Field(Field = "card_no")]
        public string CardNo { get; set; }

        [FISCA.UDT.Field(Field = "ref_student_id")]
        public string StudentID { get; set; }

        [FISCA.UDT.Field(Field = "cell_phone")]
        public string CellPhone { get; set; }
    }
}
