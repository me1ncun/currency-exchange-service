
namespace project;

public class ExchangeRateDTO
{
    public int ID { get; set; }
    public Currency BaseCurrency { get; set; }
    public Currency TargetCurrency { get; set; }
    public decimal Rate { get; set; }

    public ExchangeRateDTO() { }
}
