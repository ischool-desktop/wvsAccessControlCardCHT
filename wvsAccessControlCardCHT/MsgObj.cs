using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wvsAccessControlCardCHT
{
    class MsgObj
    {
        private string arrive_msg, leave_msg, error_phone;

        private bool enable_arrive, enable_leave, enable_error;

        public MsgObj(string arrive_msg, string leave_msg, string error_phone, bool enable_arrive,bool enable_leave,bool enable_error)
        {
            this.arrive_msg = arrive_msg;
            this.leave_msg = leave_msg;
            this.error_phone = error_phone;
            this.enable_arrive = enable_arrive;
            this.enable_leave = enable_leave;
            this.enable_error = enable_error;
        }

        public string Arrive_msg
        {
            get { return arrive_msg; }
            set { arrive_msg = value; }
        }

        public string Leave_msg
        {
            get { return leave_msg; }
            set { leave_msg = value; }
        }

        public string Error_phone
        {
            get { return error_phone; }
            set { error_phone = value; }
        }
        
        public bool Enable_arrive
        {
            get { return enable_arrive; }
            set { enable_arrive = value; }
        }

        public bool Enable_leave
        {
            get { return enable_leave; }
            set { enable_leave = value; }
        }

        public bool Enable_error
        {
            get { return enable_error; }
            set { enable_error = value; }
        }
    }
}
