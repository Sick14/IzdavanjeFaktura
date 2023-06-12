using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IzdavanjeFaktura.Models;
using Microsoft.AspNet.Identity;

namespace IzdavanjeFaktura.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invoices
        public ActionResult Index(int? page = 1, string number = "")
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.Number = number;

            IQueryable<Invoice> invoices = db.Invoices;

            if (!String.IsNullOrEmpty(number))
            {
                invoices = invoices.Where(i => i.InvoiceNumber.ToLower().Contains(number.ToLower()));
            }

            ViewBag.PageCount = Convert.ToInt32(Math.Ceiling(invoices.Count() / (double)pageSize));

            IQueryable<Invoice> pagedResults = invoices.OrderByDescending(i => i.InvoiceID).Skip((pageNumber - 1) * pageSize).Take(pageSize);

            List<InvoiceViewModel> invoiceList = new List<InvoiceViewModel>();

            foreach (var item in pagedResults)
            {
                InvoiceViewModel invoiceViewModel = new InvoiceViewModel();

                invoiceViewModel.TotalPriceWithoutVAT = item.TotalPriceWithoutVAT;
                invoiceViewModel.InvoiceNumber = item.InvoiceNumber;
                invoiceViewModel.InvoiceIssueDate = item.InvoiceIssueDate.Date;
                invoiceViewModel.InvoiceDueDate = item.InvoiceDueDate.Date;
                invoiceViewModel.Customer = item.Customer;
                invoiceViewModel.TotalPriceWithVAT = item.TotalPriceWithVAT;
                invoiceViewModel.InvoiceID = item.InvoiceID;
                invoiceViewModel.User = db.Users.Where(u => u.Id == item.UserID).FirstOrDefault();
                //invoiceViewModel.InvoiceItems = item.InvoiceItems.ToList();

                invoiceList.Add(invoiceViewModel);
            }

            return View(invoiceList);
        }

        // GET: Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);

            InvoiceViewModel invoiceViewModel = new InvoiceViewModel();

            invoiceViewModel.TotalPriceWithoutVAT = invoice.TotalPriceWithoutVAT;
            invoiceViewModel.InvoiceNumber = invoice.InvoiceNumber;
            invoiceViewModel.InvoiceIssueDate = invoice.InvoiceIssueDate.Date;
            invoiceViewModel.InvoiceDueDate = invoice.InvoiceDueDate.Date;
            invoiceViewModel.Customer = invoice.Customer;
            invoiceViewModel.TotalPriceWithVAT = invoice.TotalPriceWithVAT;
            invoiceViewModel.InvoiceID = invoice.InvoiceID;
            invoiceViewModel.User = db.Users.Where(u => u.Id == invoice.UserID).FirstOrDefault();
            invoiceViewModel.InvoiceItems = invoice.InvoiceItems.ToList();

            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoiceViewModel);
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
                    TotalPriceWithVAT = invoiceViewModel.TotalPriceWithVAT,
                    UserID = User.Identity.GetUserId()
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
                                                         Text = p.Description
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

            InvoiceViewModel invoiceViewModel = new InvoiceViewModel();

            invoiceViewModel.TotalPriceWithoutVAT = invoice.TotalPriceWithoutVAT;
            invoiceViewModel.InvoiceNumber = invoice.InvoiceNumber;
            invoiceViewModel.InvoiceIssueDate = invoice.InvoiceIssueDate.Date;
            invoiceViewModel.InvoiceDueDate = invoice.InvoiceDueDate.Date;
            invoiceViewModel.Customer = invoice.Customer;
            invoiceViewModel.TotalPriceWithVAT = invoice.TotalPriceWithVAT;
            invoiceViewModel.InvoiceID = invoice.InvoiceID;
            invoiceViewModel.User = db.Users.Where(u => u.Id == invoice.UserID).FirstOrDefault();

            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoiceViewModel);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                List<InvoiceItem> invocieItems = db.InvoiceItems.Where(i => i.InvoiceID == id).ToList();
                db.InvoiceItems.RemoveRange(invocieItems);
                db.SaveChanges();

                Invoice invoice = db.Invoices.Find(id);
                db.Invoices.Remove(invoice);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

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
