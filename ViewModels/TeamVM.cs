using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using u17120323_INF354_HW1.Models;

namespace u17120323_INF354_HW1.ViewModels
{
    public class TeamVM
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public double TeamAverage { get; set; }
        public List<PlayerVM> TeamPlayers = new List<PlayerVM>();
    }
}