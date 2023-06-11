using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IzdavanjeFaktura.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Description { get; set; }
        [Display(Name = "Price (Without VAT)")]
        public decimal PriceWithoutVAT { get; set; }
    }
}