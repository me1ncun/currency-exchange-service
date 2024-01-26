
using Exchange.Database.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Net;
using System.Xml.Linq;

namespace project;

[ApiController]
[Route("[controller]")]
public class CurrenciesController : ControllerBase
{
    private CurrenciesService currenciesService;

    public CurrenciesController(CurrenciesService currenciesService)
    {
        this.currenciesService = currenciesService;
    }

    [HttpGet("/currencies")]
    public IActionResult Get()
    {
        var currencies = currenciesService.GetAllCurrencies();
        if (currencies != null)
        {
            return new JsonResult(currencies);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("/currency/{code}")]
    public IActionResult GetByСode(string code)
    {
        var currency = currenciesService.GetCurrency(code);
        if (currency != null)
        {
            return new JsonResult(currenciesService.GetCurrency(code));
        }
        else if (currency == null)
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

    [HttpPost("/currencies")]
    public IActionResult Post([FromBody]Currency currency)
    {
        var addedCurrency = currenciesService.PostCurrency(currency.Name, currency.Code, currency.Sign);

        if (addedCurrency != null)
        {
            return new JsonResult(addedCurrency);
        }
        else if (currency.Code == null || currency.Name == null || currency.Sign == null)
        {
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}