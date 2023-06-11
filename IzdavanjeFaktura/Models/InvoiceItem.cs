using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IzdavanjeFaktura.Models
{
    public class InvoiceItem
    {
        public int InvoiceItemID { get; set; }
        public virtual Product Product { get; set; }
        public int ProductID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int Quantity { get; set; }
        [Display(Name = "Item price (Without VAT)")]
        public decimal PriceWithoutVAT { get; set; }
        [Display(Name = "Total item price (Without VAT)")]
        public decimal TotalPriceWithoutVAT { get; set; }
        public int? InvoiceID { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}