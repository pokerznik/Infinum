using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Infinum.ZanP.Core.Interfaces;
using Infinum.ZanP.Core.Models.SQL;
using Infinum.ZanP.Infrastructure.Data;
using Infinum.ZanP.Infrastructure.Services.Repositories;

namespace Infinum.ZanP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private IContactService m_contactService;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IContactService p_contactService
            )
        {
            _logger = logger;
            m_contactService = p_contactService;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            Country c = new Country()
            {
                Id = 203,
                Name = "Slovenia"
            };

            //var cnts = await m_contactService.GetAllCountries();


          /* Contact cnt = new Contact()
            {
                Address = new Address("Zgornja Bistrica", "96", "2310", "Slovenska Bistrica", c),
                DateOfBirth = new DateTime(1996, 08, 21),
                Name = "Zan Pokerznik",
                TelephoneNumbers = new List<TelephoneNumber>()
                {
                    new TelephoneNumber(386, 40716065)
                }
            };

            var created = await m_contactService.Create(cnt);*/

            Contact zan = new Contact()
            {
                Id = 1,
                Name= "Lori",
                DateOfBirth = new DateTime(2018, 4, 10)
            };
            var update = await m_contactService.Update(zan);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
