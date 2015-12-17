using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6.Models;

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class WineController : Controller
    {
        private BeverageJAckermanEntities db = new BeverageJAckermanEntities();

        // GET: /Wine/
        public ActionResult Index()
        {
            DbSet<Beverage> WineToSearch = db.Beverages;

            string filterName = "";
            string filterPack = "";
            string filterMin = "";
            string filterMax = "";
            string filterActive = "";

            decimal min = 0;
            decimal max = 9999.99m;

            bool activeBool = false;

            //Check to see there is a value in the session, and if there is, assign it to the
            //variable that we setup to hold the value.
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["name"];
            }
            if(Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
            }
            //same as above but for min, and we are parsing the string
            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                min = Decimal.Parse(filterMin);
            }
            //same as above but for max, and we are parsing the string
            if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMax = (string)Session["max"];
                max = Decimal.Parse(filterMax);
            }

            if (Session["active"] != null && !String.IsNullOrWhiteSpace((string)Session["active"]))
            {
                filterActive = (string)Session["active"];
                activeBool = bool.Parse(filterActive);
            }

            IEnumerable<Beverage> filtered = WineToSearch.Where(beverage => beverage.name.Contains(filterName) &&
                beverage.pack.Contains(filterPack) &&
                beverage.price >= min &&
                beverage.price <= max &&
                beverage.active == activeBool);

            IEnumerable<Beverage> filteredList = filtered.ToList();

            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;
            ViewBag.filterActive = filterActive;

            return View(filteredList);
        }

        // GET: /Wine/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: /Wine/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Wine/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Beverages.Add(beverage);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //Throws out an error if the ID the user tried to use for their new wine already exists.
                catch(DataException)
                {
                    ModelState.AddModelError("", "Sorry, that ID already exists. Please enter a different one and try again.");
                }
            }

            return View(beverage);
        }

        // GET: /Wine/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: /Wine/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: /Wine/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: /Wine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost, ActionName("Filter")]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            //Get the form data that was sent out of the Request object.
            //The string that is used as a key to get the data matches the
            //name property of the form control.
            String name = Request.Form.Get("name");
            String pack = Request.Form.Get("pack");
            String min = Request.Form.Get("min");
            String max = Request.Form.Get("max");
            String active = Request.Form.Get("active");

            //Store the form data into the session so that it can be retrived later
            //on to filter the data.
            Session["name"] = name;
            Session["pack"] = pack;
            Session["min"] = min;
            Session["max"] = max;
            Session["active"] = active;

            //Redirect the user to the index page. We will do the work of actually
            //fiiltering the list in the index method.
            return RedirectToAction("Index");
        }
    }
}
