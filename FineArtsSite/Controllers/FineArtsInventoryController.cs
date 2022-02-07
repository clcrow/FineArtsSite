using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FineArtsSite.HelperMethods;
using FineArtsSite.Models;
using System.Data;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;
using ExcelDataReader;
using System.Security.Principal;

namespace FineArtsSite.Controllers
{
    public class FineArtsInventoryController : Controller
    {
        public IActionResult Index()
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            string conn = Database.GetConnection();
            DataTable results = Database.PullInventorybyArtist(conn, "Anderson Vera");
            DataTable artList = Database.PullInventoryUniqArtist(conn);
            InventorySearchModel model = new InventorySearchModel();
            model.ArtistName = Database.getArtists(artList, "Anderson Vera");
            model.results = results;
            return View(model);
        }

        // GET: Categories/Create
        [Route("Inventory/Create")]
        public ActionResult Create()
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            return View();
        }

        [Route("Inventory/Load")]
        public ActionResult Load()
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            return View();
        }

        // POST: Inventory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Inventory/Create")]
        public ActionResult Create(Inventory inv)
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            string conn = Database.GetConnection();
            Database.addtoDB(inv, conn);
            return RedirectToAction("Index");
        }

        [HttpPost("FileUpload")]
        public ActionResult Upload(List<IFormFile> files)
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            string conn = Database.GetConnection();
            DataTable invTable = new DataTable();
            List<Inventory> inv = new List<Inventory>();
            foreach (IFormFile file in files)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        var conf = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true,
                                FilterRow = rowReader => rowReader.Depth > 5
                            }
                        };

                        var dataSet = reader.AsDataSet(conf);

                        // Now you can get data from each sheet by its index or its "name"
                        invTable = dataSet.Tables[0];
                    }
                }
            }
            foreach (DataRow row in invTable.Rows)
            {
                if (String.IsNullOrEmpty(row[1].ToString()))
                {
                    break;
                }
                Database.InInventory(row, conn);
            }
            return RedirectToAction("Index");
        }

        [Route("Inventory/Edit/{id}")]
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
            Inventory result = Database.PullInventorybyID(conn, id);
            return View(result);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Inventory/Edit/{id}")]
        public ActionResult Edit(Inventory inv, int id)
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            string conn = Database.GetConnection();
            inv.recID = id;
            Database.updateDB(inv, id, conn);
            return RedirectToAction("Index");
        }

        // GET: Categories/Delete/5
        [Route("Inventory/Delete/{id}")]
        public ActionResult Delete(int id)
        {
            var cart = Session.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.cartAmount = cart.Count;
            }
            string conn = Database.GetConnection();
            Database.deleteDB(id, conn);
            return RedirectToAction("Index");
        }

        [Route("Inventory/ArtistSearch/{name}")]
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
    }
}