using Exchange.DTO;

namespace Exchange.Database.Services
{
    public class ExchangeService
    {
        private ExchangeRatesService exchangeService;
        private CurrenciesService currencyService;
        public ExchangeService(ExchangeRatesService exchangeService, CurrenciesService currencyService) 
        {
            this.currencyService = currencyService;
            this.exchangeService = exchangeService;
        }

        public ExchangeDTO GetExchange(string baseCode, string targetCode, decimal amount)
        {
            ExchangeDTO exchangeDTO = new ExchangeDTO();
            exchangeDTO.BaseCurrency = currencyService.GetCurrency(baseCode);
            exchangeDTO.TargetCurrency = currencyService.GetCurrency(targetCode);
            exchangeDTO.Amount = Math.Round(amount, 2);
            if (exchangeService.GetExchangeRateByCode(baseCode + targetCode).Rate != 0)
            {
                exchangeDTO.Rate = Math.Round(exchangeService.GetExchangeRateByCode(baseCode + targetCode).Rate, 2);
            }
            else if (exchangeService.GetExchangeRateByCode(targetCode + baseCode).Rate != 0)
            {
                exchangeDTO.Rate = Math.Round(1 / exchangeService.GetExchangeRateByCode(targetCode + baseCode).Rate, 2);
            }
            exchangeDTO.ConvertedAmount = Math.Round((amount * exchangeDTO.Rate), 2);

            return exchangeDTO;
        }
    }
}
