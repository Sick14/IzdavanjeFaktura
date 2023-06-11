using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IzdavanjeFaktura.Models;

namespace IzdavanjeFaktura.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invoices
        public ActionResult Index()
        {
            return View(db.Invoices.ToList());
        }

        // GET: Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoices/Create
        public ActionResult Create()
        {
            var products = db.Products.ToList();
            IEnumerable<SelectListItem> selectList = from p in products
                                                     select new SelectListItem
                                                     {
                                                         Value = p.ProductID.ToString(),
                                                         Text = $"{p.Description}"
                                                     };

            ViewBag.Products = new SelectList(selectList, "Value", "Text");

            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(InvoiceViewModel invoiceViewModel)
        {
            if(invoiceViewModel.TotalPriceWithVAT == 0)
            {
                invoiceViewModel.TotalPriceWithVAT = invoiceViewModel.TotalPriceWithoutVAT * (decimal)1.17;
                ModelState.Remove("TotalPriceWithVAT");
            }
            if (ModelState.IsValid)
            {
                Invoice invoice = new Invoice()
                {
                    Customer = invoiceViewModel.Customer,
                    InvoiceDueDate = invoiceViewModel.InvoiceDueDate,
                    InvoiceIssueDate = invoiceViewModel.InvoiceIssueDate,
                    InvoiceNumber = invoiceViewModel.InvoiceNumber,
                    TotalPriceWithoutVAT = invoiceViewModel.TotalPriceWithoutVAT,
                    TotalPriceWithVAT = invoiceViewModel.TotalPriceWithVAT
                };

                db.Invoices.Add(invoice);
                db.SaveChanges();

                foreach (var item in invoiceViewModel.InvoiceItems)
                {
                    item.InvoiceID = invoice.InvoiceID;
                    db.InvoiceItems.Add(item);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            var products = db.Products.ToList();
            IEnumerable<SelectListItem> selectList = from p in products
                                                     select new SelectListItem
                                                     {
                                                         Value = p.ProductID.ToString(),
                                                         Text = $"{p.Description}"
                                                     };

            ViewBag.Products = new SelectList(selectList, "Value", "Text");

            return View(invoiceViewModel);
        }

        // GET: Invoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InvoiceID,InvoiceNumber,InvoiceIssueDate,InvoiceDueDate,TotalPriceWithoutVAT,TotalPriceWithVAT,Customer")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoices.Find(id);
            db.Invoices.Remove(invoice);
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
    }
}
