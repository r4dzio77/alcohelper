using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlcoHelper.Data;
using AlcoHelper.Models;

namespace AlcoHelper.Controllers.Api
{
    [Route("api/alcohols")]
    [ApiController]
    public class AlcoholsApiController : ControllerBase
    {
        private readonly AlcoHelperContext _context;

        public AlcoholsApiController(AlcoHelperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAlcohols()
        {
            var alcohols = await _context.Alcohols
                .Where(a => a.IsApproved) // tylko zatwierdzone
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Type,
                    a.Country,
                    a.AlcoholPercentage,
                    a.ImageUrl
                })
                .ToListAsync();

            return Ok(alcohols);
        }


        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("API działa");
        }

    }
}
