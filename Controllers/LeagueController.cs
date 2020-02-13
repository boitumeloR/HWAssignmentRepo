using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using u17120323_INF354_HW1.Models;
using u17120323_INF354_HW1.ViewModels;

namespace u17120323_INF354_HW1.Controllers
{
    public class LeagueController : ApiController
    {
        SoccerLeagueEntities db = new SoccerLeagueEntities();
        // GET: League
        //QuestionOne
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/League/GetPlayers")]
        public List<dynamic> GetPlayers()
        {
            var players = db.Players.ToList();
            return GetDynamicPlayers(players);
        }

        public List<dynamic> GetDynamicPlayers(List<Player> inplayer)
        {
            var dynList = new List<dynamic>();
            foreach (var pl in inplayer)
            {
                dynamic dynplayer = new ExpandoObject();
                dynplayer.PlayerID = pl.PlayerID;
                dynplayer.PlayerName = pl.PlayerName;
                dynplayer.PlayerSurname = pl.PlayerSurname;
                dynplayer.PlayerAge = pl.PlayerAge;
                dynplayer.TeamID = pl.TeamID;
                dynplayer.PlayerAverage = pl.PlayerAverage;

                dynList.Add(dynplayer);
            }
            return dynList;
        }

        //Question Two
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/League/GetTeamList")]
        public List<TeamVM> GetTeamList()
        {
            var returnList = new List<TeamVM>();
            var team = db.Teams.ToList();
            foreach (var tm in team)
            {
                var vm = new TeamVM();
                vm.TeamID = tm.TeamID;
                vm.TeamName = tm.TeamName;
                vm.TeamAverage = (double)tm.TeamAverage;
                foreach(var pl in tm.Players)
                {
                    // var player = tm.Players.Where(zz => zz.TeamID == vm.TeamID).FirstOrDefault();
                    var vm2 = new PlayerVM
                    {
                        PlayerID = pl.PlayerID,
                        PlayerName = pl.PlayerName,
                        PlayerSurname = pl.PlayerSurname,
                        PlayerAverage = (double)pl.PlayerAverage,
                        PlayerAge = pl.PlayerAge
                    };

                    vm.TeamPlayers.Add(vm2);
                }
                returnList.Add(vm);
            }
            return returnList;
        }

        // Question 3 
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/InsertTeam")]
        public dynamic InsertTeam([FromBody] TeamVM inTeam)
        {
            var team = new Team
            {
                LeagueID = 1,
                TeamName = inTeam.TeamName,
                TeamAverage = (decimal)inTeam.TeamAverage
            };

            db.Teams.Add(team);
            db.SaveChanges();
            var latestteam = db.Teams.ToList().OrderByDescending(zz=>zz.TeamID).FirstOrDefault();
            dynamic message = new ExpandoObject();
            if (latestteam != null)
            {
                foreach(var player in inTeam.TeamPlayers)
                {
                    var inPlayer = new Player
                    {
                        PlayerName = player.PlayerName,
                        PlayerSurname = player.PlayerSurname,
                        PlayerAge = player.PlayerAge,
                        PlayerAverage = (decimal)player.PlayerAverage,
                        TeamID = latestteam.TeamID
                    };

                    try
                    {
                        db.Players.Add(inPlayer);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        message.Success = false;
                        message.Error = e.Message;
                    }
                }
                message.Success = true;
                message.Error = null;
            }

            return message;
        }
        //Question 4

        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("api/League/UpdateLeague")]
        public dynamic UpdateLeague([FromBody] LeagueVM liga)
        {
            var league = db.Leagues.Where(zz => zz.LeagueID == liga.LeagueID).FirstOrDefault();
            dynamic message = new ExpandoObject();
            try
            {
                league.LeagueName = liga.LeagueName;
                league.LeagueLevel = Convert.ToInt32(liga.LeagueLevel);
                db.SaveChanges();
                message.success = true;
                message.error = null;
            }
            catch (Exception e)
            {
                message.success = false;
                message.error = e.Message;
            }
            return message;
        }

        //Question 5
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/League/DeletePlayer")]
        public dynamic DeletePlayer(int id)
        {
            var player = db.Players.Where(zz => zz.PlayerID == id).FirstOrDefault();
            dynamic message = new ExpandoObject();
            if(player != null)
            {
                try
                {
                    db.Players.Remove(player);
                    db.SaveChanges();

                    message.success = true;
                    message.error = null;
                }
                catch (Exception e)
                {
                    message.success = false;
                    message.error = e.Message;
                }
            }
            else
            {
                message.success = false;
                message.error = "Null parameter, try another ID";
            }
            return message;
        }
    }
}