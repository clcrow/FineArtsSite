using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FineArtsSite.HelperMethods;
using FineArtsSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace FineArtsSite.Controllers
{
    public class LoadController : Controller
    {
        public IActionResult Index()
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cartAmount = cart.Count;
            return View();
        }
    }
}