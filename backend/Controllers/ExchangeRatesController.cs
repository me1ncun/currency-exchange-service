
using Exchange.Database.Repositories;
using Exchange.Database.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace project;

[Route("[controller]")]
[ApiController]
public class ExchangeRatesController : ControllerBase
{
    private ExchangeRatesService exchangeService;
    private CurrenciesService currencyService;

    public ExchangeRatesController(ExchangeRatesService exchangeService, CurrenciesService currenciesService)
    {
        this.currencyService = currenciesService;
        this.exchangeService = exchangeService;
    }

    [HttpGet("/exchangeRates")]
    public IActionResult Get()
    {
        var exchangeRates = exchangeService.GetAllExchanges();

        if (exchangeRates != null && exchangeRates.Any())
        {
            return new JsonResult(exchangeRates);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("/exchangeRate/{code}")]
    public IActionResult GetByCode(string code)
    {
        var exchangeRates = exchangeService.GetExchangeRateByCode(code);
        if (exchangeRates != null && exchangeRates.Rate != 0)
        {
            return new JsonResult(exchangeRates);
        }
        else if(exchangeRates.Rate == 0)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
        else if(code == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("/exchangeRates")]
    public IActionResult Post( string baseCurrencyCode, string targetCurrencyCode, decimal rate)
    {
        var ex = exchangeService.PostExchangeRate(baseCurrencyCode, targetCurrencyCode, rate);
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(baseCurrencyCode);
        stringBuilder.Append(targetCurrencyCode);
        var allCurrencies = exchangeService.GetAllExchanges();

        if (allCurrencies != null)
        {
            return new JsonResult(ex);
        }
        else if(baseCurrencyCode == null || targetCurrencyCode == null || rate == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        else if (exchangeService.GetExchangeRateByCode(stringBuilder.ToString()) != null)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }
        else if (currencyService.GetCurrency(baseCurrencyCode) == null || currencyService.GetCurrency(targetCurrencyCode) == null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPatch("/exchangeRate/{CodePair}")]
    public IActionResult PatchExchangeRate(decimal rate, string codePair)
    {
        var ex = exchangeService.UpdateExchangeRate(codePair, rate);

        if(ex != null)
        {
            return new JsonResult(ex);
        }
        else if(rate == null || codePair == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        else if (exchangeService.GetExchangeRateByCode(codePair) == null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}




