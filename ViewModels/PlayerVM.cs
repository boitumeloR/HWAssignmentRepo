using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u17120323_INF354_HW1.ViewModels
{
    public class PlayerVM
    {
        public int PlayerID { get; set; }
        public string PlayerName { get; set; }
        public string PlayerSurname { get; set; }
        public double PlayerAverage { get; set; }
        public string PlayerAge { get; set; }
        public int TeamID { get; set; }
    }
}