using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IzdavanjeFaktura.Models
{
    public class Invoice
    {
        public int InvoiceID { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceIssueDate { get; set; }
        public DateTime InvoiceDueDate { get; set; }
        public decimal TotalPriceWithoutVAT { get; set; }
        public decimal TotalPriceWithVAT { get; set; }
        public string Customer { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; }
    }
}