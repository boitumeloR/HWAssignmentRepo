using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW01_API.ViewModels
{
    public class VIewUserTypeVM
    {
        public UserSession Session { get; set; }
        public List<UserTypeVM_> UserTypes = new List<UserTypeVM_>();
    }
}