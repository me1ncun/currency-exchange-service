using Exchange.Database.Repositories;
using Exchange.Database.Services;
using Exchange.DTO;
using Microsoft.AspNetCore.Mvc;
using project;

namespace Exchange.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeController : ControllerBase
    {
        private ExchangeService exchangeService; 

        public ExchangeController(ExchangeService exchangeService)
        {
           this.exchangeService = exchangeService;
        }

        [HttpGet("/exchange/from={baseCode}&to={targetCode}&amount={amount}")]
        public IActionResult GetExchange(string baseCode, string targetCode, decimal amount)
        {
            var exchangeDTO = exchangeService.GetExchange(baseCode, targetCode, amount);

            if (exchangeDTO != null)
            {
                return new JsonResult(exchangeDTO);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
