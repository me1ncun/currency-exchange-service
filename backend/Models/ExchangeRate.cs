
namespace project
{
    public class ExchangeRate
    {
        public int ID { get; set; }
        public int BaseCurrencyID { get; set; }
        public int TargetCurrencyID { get; set; }
        public decimal Rate { get; set; }

        public ExchangeRate() { }
    }
}
