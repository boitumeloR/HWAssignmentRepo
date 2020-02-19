using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HW01_API.Models;
using HW01_API.ViewModels;
using u17120323_INF354_HW1.ViewModels;

namespace u17120323_INF354_HW1.Controllers
{
    public class LeagueController : ApiController
    {
        SoccerLeagueEntities db = new SoccerLeagueEntities();
        // GET: League
        //QuestionOne
        // From what i gather every 'Get' method which needs some sort of user authentication should actually be a post method because we're passing  session every time
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/GetPlayers")]
        public dynamic GetPlayers([FromBody] UserSession sess)
        {
            if (sess != null)
            {
                AuthVM vm = new AuthVM();
                var session = vm.RefreshSession(sess);
                var players = db.Players.ToList();
                if (session.Error != null)
                {
                    return session;
                }
                else
                {
                    return GetDynamicPlayers(players, session);
                }
            }
            else
            {
                return null;
            }

        }

        public dynamic GetDynamicPlayers(List<Player> inplayer, UserSession sess)
        {
            dynamic retDyn = new ExpandoObject();

            retDyn.Session = new UserSession
            {
                SessionID = sess.SessionID,
                UserID = sess.UserID,
                Type = sess.Type,
                Error = sess.Error
            };
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

            retDyn.Players = dynList;
            return retDyn;
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
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/DeletePlayer")]
        public UserSession DeletePlayer([FromBody] SecurePlayer play)
        {
            var player = db.Players.Where(zz => zz.PlayerID == play.player.PlayerID).FirstOrDefault();
            AuthVM vm = new AuthVM();
            var sess = vm.RefreshSession(play.session);
            if (sess.Error == null)
            {
                try
                {
                    db.Players.Remove(player);
                    db.SaveChanges();

                    return sess;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                return sess;
            }
        }
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/League/GetTeams")]
        
        public List<dynamic> GetTeams()
        {
            var teams = db.Teams.ToList();
            return DynTeams(teams);
        }

        public List<dynamic> DynTeams(List<Team> teams)
        {
            var list = new List<dynamic>();

            foreach (var tm in teams)
            {
                dynamic inteam = new ExpandoObject();

                inteam.TeamID = tm.TeamID;
                inteam.TeamName = tm.TeamName;
                inteam.TeamAverage = tm.TeamAverage;

                list.Add(inteam);
            }

            return list;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/GetSecureTeams")]

        public dynamic GetSecureTeam([FromBody]UserSession session)
        {
            AuthVM vm = new AuthVM();
            var sess = vm.RefreshSession(session);
            if (sess.Error ==null)
            {
                var teams = db.Teams.ToList();
                return DynTeams(teams, sess);
            }
            else
            {
                dynamic ret = new ExpandoObject();
                ret.Teams = null;
                ret.Session = sess;
                return ret;
            }
        }

        public dynamic DynTeams(List<Team> teams, UserSession sess)
        {
            var list = new List<dynamic>();
            dynamic ret = new ExpandoObject();

            ret.Session = sess;
            foreach (var tm in teams)
            {
                dynamic inteam = new ExpandoObject();

                inteam.TeamID = tm.TeamID;
                inteam.TeamName = tm.TeamName;
                inteam.TeamAverage = tm.TeamAverage;

                list.Add(inteam);
            }
            ret.Teams = list;
            return ret;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/UpdatePlayer")]

        public UserSession UpdatePlayer([FromBody] SecurePlayer player )
        {
            if (player != null)
            {
                AuthVM vm = new AuthVM();
                var sess = vm.RefreshSession(player.session);

                if (sess.Error == null)
                {
                    var play = db.Players.Where(zz => zz.PlayerID == player.player.PlayerID).FirstOrDefault();
                    
                    if (play != null)
                    {
                        try
                        {
                            play.PlayerName = player.player.PlayerName;
                            play.PlayerSurname = player.player.PlayerSurname;
                            play.PlayerAge = player.player.PlayerAge;
                            play.PlayerAverage = (decimal)player.player.PlayerAverage;
                            play.TeamID = player.player.TeamID;

                            db.SaveChanges();

                            return sess;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                else
                {
                    return sess;
                }
            }
            return null;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/AddPlayer")]

        public UserSession AddPlayer([FromBody]SecurePlayer player)
        {
            if (player != null)
            {
                AuthVM vm = new AuthVM();
                var sess = vm.RefreshSession(player.session);

                if (sess.Error == null)
                {
                    try
                    {
                        var play = new Player();

                        play.PlayerName = player.player.PlayerName;
                        play.PlayerSurname = player.player.PlayerSurname;
                        play.PlayerAge = player.player.PlayerAge;
                        play.PlayerAverage = (decimal)player.player.PlayerAverage;
                        play.TeamID = player.player.TeamID;

                        db.Players.Add(play);
                        db.SaveChanges();

                        return sess;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return sess;
                }
            }
            return null;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/GetSecureLeague")]
        public SecureLeague GetSecureLeague([FromBody] UserSession session)
        {
            var vm = new AuthVM();
            var sess = vm.RefreshSession(session);
            var secure = new SecureLeague();
            if(sess.Error == null)
            {
                var sLg = new List<LeagueVM>();
                var leagues = db.Leagues.ToList();
                foreach(var l in leagues)
                {
                    var lg = new LeagueVM();
                    lg.LeagueID = l.LeagueID;
                    lg.LeagueName = l.LeagueName;
                    lg.LeagueLevel = l.LeagueLevel;

                    sLg.Add(lg);
                }

                secure.Leagues = sLg;
                secure.Session = sess;
                return secure;
            }
            else
            {
                secure.Session = sess;
                secure.Leagues = null;
                return secure;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/AddTeam")]
        public UserSession AddTeam([FromBody]SecureTeam team)
        {
            if (team != null)
            {
                var vm = new AuthVM();
                var sess = vm.RefreshSession(team.Session);

                if (sess.Error == null)
                {
                    var inteam = new Team
                    {
                        TeamName = team.Team.TeamName,
                        TeamAverage = (decimal)team.Team.TeamAverage,
                        LeagueID = team.Team.LeagueID
                    };
                    try
                    {
                        db.Teams.Add(inteam);
                        db.SaveChanges();

                        return sess;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    return sess;
                }
            }
            else
            {
                return null;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/AddLeague")]

        public UserSession AddLeague([FromBody]LeagueRestrict league)
        {
            var vm = new AuthVM();
            var sess = vm.RefreshSession(league.Session);

            if (sess.Error == null)
            {
                var inLeague = new League
                {
                    LeagueName = league.League.LeagueName,
                    LeagueLevel = league.League.LeagueLevel
                };

                try
                {
                    db.Leagues.Add(inLeague);
                    db.SaveChanges();

                    return sess;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return sess;
            }

        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/UpdateLeague")]

        public UserSession UpdateLeague([FromBody]LeagueRestrict league)
        {
            var vm = new AuthVM();
            var sess = vm.RefreshSession(league.Session);

            if (sess.Error == null)
            {
                var upLeague = db.Leagues.Where(zz => zz.LeagueID == league.League.LeagueID).FirstOrDefault();

                try
                {
                    upLeague.LeagueName = league.League.LeagueName;
                    upLeague.LeagueLevel = league.League.LeagueLevel;

                    db.SaveChanges();

                    return sess;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return sess;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/UpdateTeam")]

        public UserSession UpdateTeam([FromBody]SecureTeam team)
        {
            var vm = new AuthVM();
            var sess = vm.RefreshSession(team.Session);

            if(sess.Error == null)
            {
                var upteam = db.Teams.Where(zz => zz.TeamID == team.Team.TeamID).FirstOrDefault();

                try
                {
                    upteam.TeamName = team.Team.TeamName;
                    upteam.TeamAverage = (decimal)team.Team.TeamAverage;
                    upteam.LeagueID = team.Team.LeagueID;

                    db.SaveChanges();

                    return sess;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return sess;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/GetUserTypes")]

        public VIewUserTypeVM GetUserTypes([FromBody]UserSession session)
        {
            var vm = new AuthVM();
            var sess = vm.RefreshSession(session);
            var uVM = new VIewUserTypeVM();
            if (sess.Error == null)
            {

                var uList = new List<UserTypeVM_>();
                uVM.Session = sess;
                var types = db.UserRoles.ToList();

                foreach(var type in types)
                {
                    var role = new UserTypeVM_();

                    role.UserTypeID = type.UserRoleID;
                    role.UserTypeDescription = type.UserRoleDescription;

                    uList.Add(role);
                }

                uVM.UserTypes = uList;

                return uVM;
            }
            else
            {
                uVM.Session = sess;
                uVM.UserTypes = null;
                return uVM;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/AddUserType")]
        public UserSession AddUserType([FromBody] SecureUserType usertype)
        {
            var vm = new AuthVM();
            var sess = vm.RefreshSession(usertype.Session);

            if(sess.Error == null)
            {
                var role = new UserRole
                {
                    UserRoleDescription = usertype.UserType.UserTypeDescription
                };

                try
                {
                    db.UserRoles.Add(role);
                    db.SaveChanges();

                    return sess;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return sess;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/League/UpdateUserType")]

        public UserSession UpdateUserType([FromBody] SecureUserType usertype)
        {
            var vm = new AuthVM();
            var sess = vm.RefreshSession(usertype.Session);

            if (sess.Error == null)
            {
                var upUser = db.UserRoles.Where(zz => zz.UserRoleID == usertype.UserType.UserTypeID).FirstOrDefault();

                try
                {
                    upUser.UserRoleDescription = usertype.UserType.UserTypeDescription;

                    db.SaveChanges();
                    return sess;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return sess;
            }
        }
    }
}