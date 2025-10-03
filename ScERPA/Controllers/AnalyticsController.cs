using ScERPA.Models.DTOs;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;

namespace ScERPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
    
        private readonly IAnalyticsServices _analyticsServices;
        public AnalyticsController(IAnalyticsServices analyticsServices)
        {
            _analyticsServices=analyticsServices;
        }

        [HttpGet]
        [Route("Status")]
        public async Task<IActionResult> Index()
        {
            return Ok("Analytcs status ok");
        }

        [Route("TopServices")]
        public async Task<IActionResult> GetTopServices(int? first)
        {
            int top =  first ?? 10;

            TopServicesChartDto data = await _analyticsServices.GetTopServices(top);

            return Ok(data);
        }



    }
}
