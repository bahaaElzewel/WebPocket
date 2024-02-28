using Microsoft.AspNetCore.Mvc;
using WebPocket.Data;
using Microsoft.EntityFrameworkCore;
using WebPocket.DTOs.Incoming;
using Microsoft.Extensions.Localization;

namespace WebPocket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Pockets : ControllerBase
    {
        private readonly WebPocketDbContext _context;
        private readonly IStringLocalizer<Pockets> _localizer;

        public Pockets(WebPocketDbContext context, IStringLocalizer<Pockets> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        [HttpGet("Bahaa")]
        public IActionResult Bahaa()
        {
            return Ok(_localizer["ARGS", "John", "Nadia"]);
            // return Ok(_localizer.GetAllStrings());
        }

        [HttpGet("AllPockets")]
        public async Task<IActionResult> AllPockets ()
        {
            var allPockets = await _context.Pockets.Select(x => new {
                x.PocketName,
                x.CreatedAt
            }).ToListAsync();
            
            if (!allPockets.Any())
                return NotFound("No data found");
            
            return Ok(allPockets);
        }

        [HttpPost("NewPocket")]
        public async Task<IActionResult> NewPocket ([FromBody] NewPocketDTO request)
        {
            if (ModelState.IsValid)
            {
                var newPocket = new Models.Pockets {
                    PocketName = request.PocketName,
                    CreatedAt = request.CreatedAt
                };

                using var transaction = await _context.Database.BeginTransactionAsync();
                try 
                {
                    int maxId = await _context.Pockets.MaxAsync(x => x.Id);
                    await _context.Database.ExecuteSqlInterpolatedAsync($"DBCC CHECKIDENT ('Pockets', RESEED, {maxId})");
                    await _context.Pockets.AddAsync(newPocket);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(newPocket);
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while saving to the database: {ex.InnerException?.Message ?? ex.Message}");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An unexpected error occured, please try again later: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(errors);
            }
        }

        [HttpGet("GetPocketId/{pocketId}")]
        public async Task<IActionResult> GetPocketId (int pocketId)
        {
            if (pocketId <= 0)
                return BadRequest("Please provide a valid pocket id");
            
            bool isPocket = _context.Pockets.Any(x => x.Id == pocketId);
            if (!isPocket)
                return NotFound("No data found for the id you provided");
            
            var pocket = await _context.Pockets.Where(y => y.Id == pocketId).Select(x => new {
                x.PocketName,
                x.CreatedAt
            }).ToListAsync();
            return Ok(pocket);
        }
    }
}