using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace IzdavanjeFaktura.Models
{
    public class Country
    {
        public int CountryID { get; set; }
        public string Name { get; set; }
        [Display(Name = "VAT Percentage (%)")]
        public decimal VATPercentage { get; set; }
    }
}