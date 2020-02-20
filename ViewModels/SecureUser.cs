using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW01_API.ViewModels
{
    public class SecureUser
    {
        public UserSession Session { get; set; }
        public UserVM User { get; set; }
    }
}