using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using HW01_API.Models;

namespace HW01_API.ViewModels
{
    public class AuthVM
    {
        public string username { get; set; }
        public string password { get; set; }
        public int userTypeID { get; set; }


        public dynamic GenerateSession(AuthVM vm)
        {
            SoccerLeagueEntities db = new SoccerLeagueEntities();

            var checkuser = db.UserAuths.Where(zz => zz.Username == vm.username).FirstOrDefault();
            dynamic returnuser = new ExpandoObject();
            if (checkuser == null)
            {
                UserAuth auth = new UserAuth
                {
                    Username = vm.username,
                    UserPassword = ComputeSHA256(vm.password),
                    UserID = Guid.NewGuid().ToString(),
                    SessionID = Guid.NewGuid().ToString(),
                    UserRoleID = vm.userTypeID,
                    SesssionExpiry = DateTime.Now.AddMinutes(30)
                };

                try
                {
                    db.UserAuths.Add(auth);
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    throw;
                }

                var latestuser = db.UserAuths.Where(zz => zz.Username == vm.username).FirstOrDefault();
                

                returnuser.UserID = latestuser.UserID;
                returnuser.SessionID = latestuser.SessionID;
                returnuser.Type = latestuser.UserRoleID;
                returnuser.Error = null;

                return returnuser;
            }
            else
            {

                returnuser.UserID = null;
                returnuser.SessionID = null;
                returnuser.Type = null ;
                returnuser.Error = "Error! A user with the email address: "+ vm.username+ " already exists";

                return returnuser;
            }
        }

        public UserSession RefreshSession(UserSession sess)
        {
            SoccerLeagueEntities db = new SoccerLeagueEntities();
            var user = db.UserAuths.Where(zz => zz.UserID == sess.UserID && zz.SessionID == sess.SessionID).FirstOrDefault();
            UserSession ret = new UserSession(); 

            if (user != null)
            {
                if (user.SesssionExpiry > DateTime.Now)
                {
                    try
                    {
                        user.SesssionExpiry = DateTime.Now.AddMinutes(30);
                        user.SessionID = Guid.NewGuid().ToString();
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        
                    }

                    ret.SessionID = user.SessionID;
                    ret.UserID = user.UserID;
                    ret.Type = user.UserRoleID;
                    ret.Error = null;

                    return ret;
                }
                else
                {
                    ret.UserID = null;
                    ret.SessionID = null;
                    ret.Type = null;
                    ret.Error = "Session Expired, login again";
                    return ret;
                }
                
            }
            else
            {
                ret.UserID = null;
                ret.SessionID = null;
                ret.Type = null;
                ret.Error = "User doesn't exist, login again";

                return ret;
            }
        }

        public UserSession LogIn(AuthVM vm) 
        {
            SoccerLeagueEntities db = new SoccerLeagueEntities();
            var encryppass = ComputeSHA256(vm.password);
            var user = db.UserAuths.Where(zz => zz.Username == vm.username && zz.UserPassword == encryppass).FirstOrDefault();
            var ret = new UserSession();
            if (user != null) 
            {
                try
                {
                    user.UserID = Guid.NewGuid().ToString();
                    user.SessionID = Guid.NewGuid().ToString();
                    user.SesssionExpiry = DateTime.Now.AddMinutes(30);

                    db.SaveChanges();
                }
                catch (Exception)
                {

                    throw;
                }
                ret.SessionID = user.SessionID;
                ret.UserID = user.UserID;
                ret.Type = user.UserRoleID;
                ret.Error = null;

                return ret;
            }
            else
            {
                ret.SessionID = null;
                ret.UserID = null;
                ret.Type = null;
                ret.Error = "User with those credentials not found";

                return ret;
            }
        }

        public void Logout(UserSession session)
        {
            SoccerLeagueEntities db = new SoccerLeagueEntities();
            var user = db.UserAuths.Where(zz => zz.UserID == session.UserID && zz.SessionID == session.SessionID && zz.SesssionExpiry > DateTime.Now).FirstOrDefault();

            if (user != null)
            {
                try
                {
                    user.SesssionExpiry = DateTime.Now;

                    db.SaveChanges();

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public string ComputeSHA256(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}