
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

    public class ExchangePost
    {
        public string baseCurrencyCode { get; set; }
        public string targetCurrencyCode { get; set; }
        public decimal rate { get; set; }
    }

    [HttpPost("/exchangeRates")]
    public IActionResult Post([FromBody] ExchangePost exchangePost)
    {
        var ex = exchangeService.PostExchangeRate(exchangePost.baseCurrencyCode, exchangePost.targetCurrencyCode, exchangePost.rate);
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(exchangePost.baseCurrencyCode);
        stringBuilder.Append(exchangePost.targetCurrencyCode);
        var allCurrencies = exchangeService.GetAllExchanges();

        if (allCurrencies != null)
        {
            return new JsonResult(ex);
        }
        else if(exchangePost.baseCurrencyCode == null || exchangePost.targetCurrencyCode == null || exchangePost.rate == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        else if (exchangeService.GetExchangeRateByCode(stringBuilder.ToString()) != null)
        {
            return StatusCode(StatusCodes.Status409Conflict);
        }
        else if (currencyService.GetCurrency(exchangePost.baseCurrencyCode) == null || currencyService.GetCurrency(exchangePost.targetCurrencyCode) == null)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPatch("/exchangeRate/{CodePair}")]
    public IActionResult PatchExchangeRate(string codePair,decimal rate)
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
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}




