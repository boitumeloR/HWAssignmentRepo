using System;
using System.Collections.Generic;
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


        public void GenerateSession(AuthVM vm)
        {
            SoccerLeagueEntities db = new SoccerLeagueEntities();

            var checkuser = db.UserAuths.Where(zz => zz.Username == this.username).FirstOrDefault();

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
            }
            else
            {

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