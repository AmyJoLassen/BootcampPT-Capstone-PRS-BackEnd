using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsBackEnd.Data;
using PrsBackEnd.Models;

namespace PrsBackEnd.Controllers
{
    [Route("/request-lines")]
    [ApiController]


    public class RequestLinesController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public RequestLinesController(PrsDbContext context)
        {
            _context = context;
        }


        // GET: /request-lines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetAllRequestLines()
        {
            return await _context.RequestLine.ToListAsync();
        }

        // GET: /request-lines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLineId(int id)
        {
            var requestLine = await _context.RequestLine
                .Include(rl => rl.Request)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync();

            if (requestLine == null)
            {
                return NotFound();
            }

            await RecalculateRequestTotal(requestLine.RequestId);
            return requestLine;
        }

        // PUT: /request-lines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequestLine(int id, RequestLine requestLine)
        {
            if (id != requestLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(requestLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();   // update
                await RecalculateRequestTotal(requestLine.RequestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestLineExists(id))
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



        // POST: /request-lines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RequestLine>> CreateRequestLine(RequestLine requestLine)
        {
            _context.RequestLine.Add(requestLine);
            await _context.SaveChangesAsync();
            await RecalculateRequestTotal(requestLine.RequestId);

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }

        // DELETE: /requestlLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestLine(int id)
        {
            var requestLine = await _context.RequestLine.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            int theRequestId = requestLine.RequestId;

            _context.RequestLine.Remove(requestLine);
            await _context.SaveChangesAsync();
            await RecalculateRequestTotal(requestLine.RequestId);

            return NoContent();
        }

        private bool RequestLineExists(int id)
        {
            return _context.RequestLine.Any(e => e.Id == id);
        }


        private async Task RecalculateRequestTotal(int RequestId)
        {
            var total = await _context.RequestLine
                .Where(rl => rl.RequestId == RequestId)
                .Include(rl => rl.Product)
                .Select(rl => new { linetotal = rl.Product.Price * rl.Quantity })
                .SumAsync(sum => sum.linetotal);

            var requ = await _context.Request.FindAsync(RequestId);

            requ.Total = total;

            await _context.SaveChangesAsync();

        }

    }
}
