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

            return await this._context.Cities.OrderBy(c => c.CityName).ToListAsync();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<City>> GetCity([FromQuery]Guid id)
        {
            this._logger.LogInformation("{ControllerName}.{MethodName} action GET method",
                nameof(CitiesController), nameof(this.GetCity));

            var city = await this._context.Cities.FindAsync(id);

            if (city == null)
            {
                // return NotFound();
                return this.Problem(detail: "Invalid CityId", statusCode: 404, title: "City search");
            }

            return city;
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> PutCity([FromQuery] Guid id,
            [FromBody][Bind(nameof(City.CityId), nameof(City.CityName))] City city)
        {
            this._logger.LogInformation("{ControllerName}.{MethodName} action PUT method",
                nameof(CitiesController), nameof(this.PutCity));

            if (id != city.CityId)
            {
                // return BadRequest(); //HTTP 400
                return this.Problem(detail: "id and city.CityId are not match", 
                    statusCode: 400, title: "Update city");
            }

            //this._context.Entry(city).State = EntityState.Modified;
            var cityFromDb = await this._context.Cities.FirstOrDefaultAsync(c => c.CityId == id);

            if(cityFromDb == null)
            {
                return this.NotFound(); //HTTP 404
            }

            cityFromDb.CityName = city.CityName;

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
        public async Task<ActionResult<City>> PostCity
            ([FromBody][Bind(nameof(City.CityId), nameof(City.CityName))]City city)
        {
            this._logger.LogInformation("{ControllerName}.{MethodName} action POST method",
                nameof(CitiesController), nameof(this.PostCity));

            this._context.Cities.Add(city);
            await this._context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = city.CityId }, city);
        }

        [HttpDelete]
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
