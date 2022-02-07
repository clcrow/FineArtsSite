using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FineArtsSite.Models;
using System.Data;
using FineArtsSite.HelperMethods;

namespace FineArtsSite.Controllers
{
    public class SalesController : Controller
    {
        public IActionResult Index(string name)
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            string conn = Database.GetConnection();
            DataTable results = Database.PullInventorybyArtist(conn, "Anderson Vera");
            DataTable artList = Database.PullInventoryUniqArtist(conn);
            InventorySearchModel model = new InventorySearchModel();
            model.ArtistName = Database.getArtists(artList, "Anderson Vera");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;

                foreach (var item in cart)
                {
                    int i = 0;
                    foreach (DataRow item2 in results.Rows)
                    {
                        if (item2.ItemArray[0].ToString() == item.cartInv.recID.ToString())
                        {
                            results.Rows[i].Delete();
                            results.AcceptChanges();
                            break;
                        }
                        i++;
                    }
                }
            }
            model.results = results;
            return View(model);
        }

        [Route("cart")]
        public IActionResult Cart()
        {
            CartFormModel model = new CartFormModel();
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
                model.cart = cart;
                float Sum = cart.Sum(item => item.cartInv.Cost);
                model.total = Math.Round(Sum, 2).ToString("0.00");
            }
            return View(model);
        }

        [Route("buy/{id}")]
        public IActionResult Buy(int id)
        {
            string conn = Database.GetConnection();
            Inventory inv = Database.PullInventorybyID(conn, id);
            if (Session.GetObjectFromJson<List<Inventory>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { cartInv = inv });
                Session.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                cart.Add(new Item { cartInv = inv });
                Session.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            string name = inv.ArtistName;
            return RedirectToAction("ArtistSearch", new { name = name });
        }

        private int isExist(int id)
        {
            List<Item> cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].cartInv.recID.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

        [Route("cart/remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<Item> cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cartAmount = cart.Count;
            int index = isExist(id);
            cart.RemoveAt(index);
            Session.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Cart");
        }

        [Route("Sales/ArtistSearch/{name}")]
        public IActionResult ArtistSearch(InventorySearchModel form, string name)
        {
            InventorySearchModel model = new InventorySearchModel();
            if (name == null)
            {
                model._artistName = Request.Form["ArtistName"].ToString();
            }
            else model._artistName = name;

            string conn = Database.GetConnection();

            DataTable artList = Database.PullInventoryUniqArtist(conn);
            model.ArtistName = Database.getArtists(artList, model._artistName);

            DataTable results = Database.PullInventorybyArtist(conn, model._artistName);

            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;

                foreach (var item in cart)
                {
                    int i = 0;
                    foreach (DataRow item2 in results.Rows)
                    {
                        if (item2.ItemArray[0].ToString() == item.cartInv.recID.ToString())
                        {
                            results.Rows[i].Delete();
                            results.AcceptChanges();
                            break;
                        }
                        i++;
                    }
                }
            }

            model.results = results;

            return View("index", model);
        }

        public IActionResult ArtistSearch2(InventorySearchModel form)
        {
            InventorySearchModel model = new InventorySearchModel();
            model._artistName = Request.Form["ArtistName"].ToString();

            string conn = Database.GetConnection();

            DataTable artList = Database.PullInventoryUniqArtist(conn);
            model.ArtistName = Database.getArtists(artList, model._artistName);

            DataTable results = Database.PullInventorybyArtist(conn, model._artistName);

            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;

                foreach (var item in cart)
                {
                    int i = 0;
                    foreach (DataRow item2 in results.Rows)
                    {
                        if (item2.ItemArray[0].ToString() == item.cartInv.recID.ToString())
                        {
                            results.Rows[i].Delete();
                            results.AcceptChanges();
                            break;
                        }
                        i++;
                    }
                }
            }

            model.results = results;

            return View("index", model);
        }

        public IActionResult Purchase(CartFormModel form)
        {
            string invoiceNums = "";
            string conn = Database.GetConnection();

            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            float Sum = cart.Sum(item => item.cartInv.Cost);
            string sumString = Math.Round(Sum, 2).ToString("0.00");

            foreach (var item in cart)
            {
                invoiceNums += item.cartInv.recID + " ";
            }

            invoiceNums.Trim();

            Database.addInvoice(invoiceNums, sumString, form.PaymentType, conn);

            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

        public IActionResult Invoices()
        {
            string conn = Database.GetConnection();
            DataTable results = Database.pullInvoices(conn);
            return View(results);
        }

        [Route("Invoices/Edit/{id}")]
        public ActionResult Edit(int id)
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            float cost;
            string sold;
            Inventory model = new Inventory();
            model.recID = id;
            string conn = Database.GetConnection();
            Invoice result = Database.pullInvoicesByID(conn, id);
            return View(result);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Invoices/Edit/{id}")]
        public ActionResult Edit(Invoice inv, int id)
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            string conn = Database.GetConnection();
            inv.recID = id;
            Database.updateInvoiceDB(inv, id, conn);
            return RedirectToAction("Invoices");
        }
    }
}