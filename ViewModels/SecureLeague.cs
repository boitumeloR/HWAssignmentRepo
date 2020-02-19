using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using u17120323_INF354_HW1.ViewModels;

namespace HW01_API.ViewModels
{
    public class SecureLeague
    {
        public List<LeagueVM> Leagues = new List<LeagueVM>();
        public UserSession Session { get; set; }
    }
}