using IzdavanjeFaktura.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using IzdavanjeFaktura.Models;

namespace IzdavanjeFaktura.Controllers
{
    public class VATController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [ImportMany]
        public IEnumerable<IVatCalculator> VatCalculators { get; set; }

        [HttpGet]
        public ActionResult CalculateVAT(int countryId, decimal price)
        {
            // Get the VAT calculator plugin based on the selected country
            decimal vatPercentage = db.Countries.Find(countryId).VATPercentage; //GetTaxPercentageByCountryId(countryId); // Implement your own logic to retrieve tax percentage by country ID

            IVatCalculator vatCalculator = new VatCalculator();
            decimal vatAmount = vatCalculator.CalculateVat(price, vatPercentage);

            return Json(vatAmount, JsonRequestBehavior.AllowGet);
        }
    }
}