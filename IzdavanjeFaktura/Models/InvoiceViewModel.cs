using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace IzdavanjeFaktura.Models
{
    public class InvoiceViewModel
    {
        [Display(Name = "Number")]
        [Required]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Issue Date")]
        public DateTime InvoiceIssueDate { get; set; }
        [Display(Name = "Due Date")]
        public DateTime InvoiceDueDate { get; set; }
        [Display(Name = "Total price (Without VAT)")]
        public decimal TotalPriceWithoutVAT { get; set; }
        [Display(Name = "Total price (With VAT)")]
        public decimal TotalPriceWithVAT { get; set; }
        public string Customer { get; set; }
        [Required]
        public List<InvoiceItem> InvoiceItems { get; set; }
        //public int ProductID { get; set; }
        //public string ProductDescription { get; set; }
    }
}