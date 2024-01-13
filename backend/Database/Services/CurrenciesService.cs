using Exchange.Database.Repositories;
using Microsoft.IdentityModel.Tokens;
using project;
using ZstdSharp.Unsafe;

namespace Exchange.Database.Services;

public class CurrenciesService
{
    private CurrenciesRepository CurrenciesRepository;

    public CurrenciesService(CurrenciesRepository CurrenciesRepository)
    {
        this.CurrenciesRepository = CurrenciesRepository;
    }

    public List<Currency> GetAllCurrencies()
    {
        return CurrenciesRepository.Read();
    }

    public Currency GetCurrency(string code)
    {
        var currency = GetAllCurrencies().FirstOrDefault(currency => currency.Code == code);

        if (currency == null)
        {
            throw new Exception("Currency not found");
        }

        return currency;
    }
    public Currency GetCurrency(int id)
    {
        var currency = GetAllCurrencies().Where(currency => currency.ID == id).ToList().First();

        if (currency == null)
        {
            throw new Exception("Currency not found");
        }

        return currency;
    }

    public Currency PostCurrency(string fullName, string code, string sign)
    {
        bool added;
        try
        {
            added = CurrenciesRepository.Create(fullName, code, sign);
        }
        catch (Exception e) 
        {
            throw new Exception("Currency already exists");
        }
        return GetCurrency(code);
    }
}
