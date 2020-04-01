using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Infinum.ZanP.Core.Interfaces;
using Infinum.ZanP.Core.Models.SQL;
using Infinum.ZanP.Core.Interfaces.Repositories;

namespace Infinum.ZanP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly IUnitOfWork m_unitOfWork;

        public CountriesController(IUnitOfWork p_unitOfWork)
        {
            m_unitOfWork = p_unitOfWork;
        }

        [HttpGet]
        public async Task<IEnumerable<Country>> GetCountries()
        {
            return await m_unitOfWork.Countries.GetAllAsync();
        }
    }
}