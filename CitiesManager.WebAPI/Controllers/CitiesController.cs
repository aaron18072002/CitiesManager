using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CitiesManager.WebAPI.DatabaseContext;
using CitiesManager.WebAPI.Models;

namespace CitiesManager.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CitiesController> _logger;

        public CitiesController(ApplicationDbContext context, ILogger<CitiesController> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [Route("")]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            this._logger.LogInformation("{ControllerName}.{MethodName} action GET method",
                nameof(CitiesController), nameof(this.GetCities));

            return await this._context.Cities.ToListAsync();
        }

        [HttpGet("{id}")]
        [Route("[action]")]
        public async Task<ActionResult<City>> GetCity(Guid id)
        {
            this._logger.LogInformation("{ControllerName}.{MethodName} action GET method",
                nameof(CitiesController), nameof(this.GetCity));

            var city = await this._context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        [HttpPut("{id}")]
        [Route("[action]")]
        public async Task<IActionResult> PutCity(Guid id, City city)
        {
            this._logger.LogInformation("{ControllerName}.{MethodName} action PUT method",
                nameof(CitiesController), nameof(this.PutCity));

            if (id != city.CityId)
            {
                return BadRequest();
            }

            this._context.Entry(city).State = EntityState.Modified;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            this._logger.LogInformation("{ControllerName}.{MethodName} action POST method",
                nameof(CitiesController), nameof(this.PostCity));

            this._context.Cities.Add(city);
            await this._context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.CityId }, city);
        }

        [HttpDelete()]
        [Route("[action]")]
        public async Task<IActionResult> DeleteCity([FromQuery]Guid id)
        {
            this._logger.LogInformation("{ControllerName}.{MethodName} action DELETE method",
                nameof(CitiesController), nameof(this.DeleteCity));

            var city = await this._context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            this._context.Cities.Remove(city);
            await this._context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(Guid id)
        {
            return this._context.Cities.Any(e => e.CityId == id);
        }
    }
}
