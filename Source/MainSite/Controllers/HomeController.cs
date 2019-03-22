using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MainSite.Models;
using NexusForever.Shared.Database.Auth;
using Microsoft.AspNetCore.Authorization;

namespace MainSite.Controllers
{
    public class HomeController : Controller
    {
        private bool IsOnline = false;

        public IActionResult Index()
        {
            IsOnline = true;
            GetStatusImage();
            return View();
        }

        [HttpPost]
        public IActionResult Register(AccountBaseModel newUser)
        {
            IsOnline = false;
            GetStatusImage();
            if (newUser.Email != null && newUser.Confirmation != null && newUser.Password != null)
            {
                if (newUser.Password.Equals(newUser.Confirmation))
                {
                    try
                    {
                        AuthDatabase.CreateAccount(newUser.Email, newUser.Password);
                        return View("RegisterSuccess");
                    }
                    catch
                    {
                        return View("DBException");
                    }
                }
                return View("RegisterFailed");
            }
            return View("Index");
        }

        private void GetStatusImage()
        {
            switch (IsOnline)
            {
                case true:
                    ViewBag.StatusSrc = "images/StatusOnline.png";
                    break;
                default:
                    ViewBag.StatusSrc = "images/StatusOffline.png";
                    break;
                    
            }
        }
    }
}
