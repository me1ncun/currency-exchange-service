using project;

namespace Exchange.DTO
{
    public class ExchangeDTO
    {
        public Currency BaseCurrency { get; set; }
        public Currency TargetCurrency { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }

        public ExchangeDTO() { }
    }
}
