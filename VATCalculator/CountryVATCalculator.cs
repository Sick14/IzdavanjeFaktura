using IzdavanjeFaktura.Interfaces;
using System.ComponentModel.Composition;

namespace VATCalculator
{
    [Export(typeof(IVatCalculator))]
    public class CountryVATCalculator : IVatCalculator
    {
        private readonly decimal vatRate;

        public CountryVATCalculator(decimal vatRate)
        {
            this.vatRate = vatRate;
        }

        public decimal CalculateVat(decimal price)
        {
            decimal vatAmount = price * vatRate;
            return vatAmount;
        }
    }
}