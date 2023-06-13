using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzdavanjeFaktura.Interfaces
{
    public interface IVatCalculator
    {
        decimal CalculateVat(decimal price, decimal vat);
    }

    public class VatCalculator : IVatCalculator
    {
        public decimal CalculateVat(decimal price, decimal vat)
        {
            // Calculate VAT based on the provided VAT percentage
            decimal vatAmount = price * (1 + (decimal)0.01 *vat);
            return vatAmount;
        }
    }
}
