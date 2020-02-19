using HW01_API.Models;
using HW01_API.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace HW01_API.Controllers
{
    public class AuthController : ApiController
    {
        SoccerLeagueEntities db = new SoccerLeagueEntities();
        // GET: Auth
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/Auth/GetUserTypes")]
        public List<dynamic> GetUserTypes()
        {
            var roles = db.UserRoles.ToList();
            return DynRoles(roles);
        }

        public List<dynamic> DynRoles(List<UserRole> roles)
        {
            var dynroles = new List<dynamic>();
            foreach (var role in roles)
            {
                dynamic rol = new ExpandoObject();

                rol.UserRoleID = role.UserRoleID;
                rol.Description = role.UserRoleDescription;

                dynroles.Add(rol);
            }

            return dynroles;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Auth/RegisterUser")]

        public dynamic RegisterUser(AuthVM vm)
        {
            if (vm != null)
            {
                var registerreturn = vm.GenerateSession(vm);
                return registerreturn;
            }
            else
            {
                dynamic registerreturn = new ExpandoObject();
                registerreturn.UserID = null;
                registerreturn.SessionID = null;
                registerreturn.Type = null;
                registerreturn.Error = "Parameters included null values";
                return registerreturn;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Auth/Login")]

        public dynamic Login([FromBody] AuthVM vm)
        {
            var session = vm.LogIn(vm);
            return session;
        }
    }
}