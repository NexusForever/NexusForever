using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MainSite.Models;
using NexusForever.Shared.Database.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Net.Sockets;
using System.Collections.Immutable;
using NexusForever.Shared.Database.Auth.Model;

namespace MainSite.Controllers
{
    public class HomeController : Controller
    {
        private bool isOnline = false;
        private ImmutableList<Server> servers;

        public IActionResult Index()
        {
            GetStatusImage();
            return View();
        }

        public IActionResult Register(AccountBaseModel newUser)
        {
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
            try
            {
                if (servers == null)
                {
                    servers = GetServers();
                }

                isOnline = PingHost(servers.First().Host, servers.First().Port);

                switch (isOnline)
                {
                    case true:
                        ViewBag.StatusSrc = "images/StatusOnline.png";
                        break;
                    default:
                        ViewBag.StatusSrc = "images/StatusOffline.png";
                        break;

                }
            }
            catch (Exception ex)
            {
                ViewBag.StatusSrc = "images/StatusOffline.png";
            }
        }

        private static bool PingHost(string hostIP, int portNr)
        {
            try
            {
                using (var client = new TcpClient(hostIP, portNr))
                {
                    return true;
                }
            }
            catch (SocketException ex)
            {
                return false;
            }
        }

        private static ImmutableList<Server> GetServers()
        {
            return AuthDatabase.GetServers();
        }
    }
}
