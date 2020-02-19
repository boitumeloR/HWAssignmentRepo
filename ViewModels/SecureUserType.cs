using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW01_API.ViewModels
{
    public class SecureUserType
    {
        public UserSession Session { get; set; }
        public UserTypeVM_ UserType { get; set; }
    }
}