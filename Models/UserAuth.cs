//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HW01_API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserAuth
    {
        public int UserAuthID { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
        public Nullable<int> UserRoleID { get; set; }
        public string UserID { get; set; }
        public string SessionID { get; set; }
        public System.DateTime SesssionExpiry { get; set; }
    
        public virtual UserRole UserRole { get; set; }
    }
}
