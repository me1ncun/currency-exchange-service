using Exchange.Database.Repositories;
using project;

namespace Exchange.Database.Services;

public class ExchangeRatesService
{
    private ExchangeRatesRepository ExchangeRatesRepository;

    public ExchangeRatesService(ExchangeRatesRepository ExchangeRatesRepository)
    {
        this.ExchangeRatesRepository = ExchangeRatesRepository;
    }

    public List<ExchangeRateDTO> GetAllExchanges()
    {
        return ExchangeRatesRepository.Read();
    }

    public ExchangeRateDTO GetExchangeRateByCode(string code)
    {
        var exchangeRateDTO = ExchangeRatesRepository.GetExchangeRateByCode(code);

        if (exchangeRateDTO == null)
        {
            throw new Exception("ExchangeRate not found");
        }

        return exchangeRateDTO;
    }

    public ExchangeRateDTO PostExchangeRate(string baseCurrencyId, string targetCurrencyId, decimal rate)
    {
        bool added;
        try
        {
            added = ExchangeRatesRepository.Create(baseCurrencyId, targetCurrencyId, rate);
        }
        catch (Exception e)
        {
            throw new Exception("Exchange already exists");
        }
        return ExchangeRatesRepository.GetExchangeRateByTwoCodes(baseCurrencyId, targetCurrencyId, rate);
    }

    public ExchangeRateDTO UpdateExchangeRate(string codePair, decimal rate)
    {
        var exchangeRateDTO =  ExchangeRatesRepository.UpdateExchangeRate(codePair, rate);

        if (exchangeRateDTO == null)
        {
            throw new Exception("ExchangeRate not found");
        }

        return exchangeRateDTO;
    }
}
