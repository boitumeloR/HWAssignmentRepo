using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW01_API.ViewModels
{
    public class UserSession
    {
        public string UserID { get; set; }
        public string SessionID { get; set; }
        public int? Type { get; set; }
        public string Error { get; set; }
    }
}