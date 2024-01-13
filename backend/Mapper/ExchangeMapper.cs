using Exchange.Database.Repositories;
using Exchange.Database.Services;
using project;

namespace Exchange.Mapper
{
    public class ExchangeMapper
    {
        private CurrenciesRepository currenciesRepository;

        public ExchangeMapper(CurrenciesRepository currenciesRepository)
        {
            this.currenciesRepository = currenciesRepository;
        }

        public ExchangeRateDTO GetTo(ExchangeRate exchangeRate) 
        {
            return new ExchangeRateDTO
            {
                ID = exchangeRate.ID,
                BaseCurrency = currenciesRepository.ReadByID(exchangeRate.BaseCurrencyID),
                TargetCurrency = currenciesRepository.ReadByID(exchangeRate.TargetCurrencyID),
                Rate = exchangeRate.Rate,
            };
        }
    }
}
