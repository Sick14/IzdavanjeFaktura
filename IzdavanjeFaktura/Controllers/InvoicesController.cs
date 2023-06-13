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
using System.Web.Services.Description;
using IzdavanjeFaktura.Models;
using Microsoft.AspNet.Identity;

namespace IzdavanjeFaktura.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invoices
        public ActionResult Index(int? page = 1, string number = "", string message = null)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            TempData["success"] = message;

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
            invoiceViewModel.Country = db.Countries.Where(c => c.CountryID == invoice.CountryID).FirstOrDefault();

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
            IEnumerable<SelectListItem> productsList = from p in products
                                                     select new SelectListItem
                                                     {
                                                         Value = p.ProductID.ToString(),
                                                         Text = p.Description
                                                     };

            ViewBag.Products = new SelectList(productsList, "Value", "Text");

            var countries = db.Countries.ToList();
            IEnumerable<SelectListItem> countriesList = from c in countries
                                                        select new SelectListItem
                                                     {
                                                         Value = c.CountryID.ToString(),
                                                         Text = c.Name + " (" + c.VATPercentage + " %)"
                                                     };

            ViewBag.Countries = new SelectList(countriesList, "Value", "Text");

            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(InvoiceViewModel invoiceViewModel)
        {
            var dateCompare = DateTime.Compare(invoiceViewModel.InvoiceIssueDate, invoiceViewModel.InvoiceDueDate);
            if(dateCompare > 0) //First Date is later than the second date
            {
                ModelState.AddModelError("InvoiceDueDate", "Invoice Due Date can't be before Invoce Issue Date");
                ModelState.AddModelError("InvoiceIssueDate", "Invoice Issue Date can't be after Invoce Due Date");
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
                    UserID = User.Identity.GetUserId(),
                    CountryID = invoiceViewModel.CountryID
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

            var countries = db.Countries.ToList();
            IEnumerable<SelectListItem> countriesList = from c in countries
                                                        select new SelectListItem
                                                        {
                                                            Value = c.CountryID.ToString(),
                                                            Text = c.Name + " (" + c.VATPercentage + " %)"
                                                        };

            ViewBag.Countries = new SelectList(countriesList, "Value", "Text");

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

                TempData["success"] = "Invoice edited successfully!";

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
            invoiceViewModel.InvoiceItems = invoice.InvoiceItems.ToList();
            invoiceViewModel.Country = db.Countries.Where(c => c.CountryID == invoice.CountryID).FirstOrDefault();

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

                TempData["success"] = "Invoice deleted successfully!";
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
