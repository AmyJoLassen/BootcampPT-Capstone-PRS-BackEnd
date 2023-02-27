using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsBackEnd.Data;
using PrsBackEnd.Models;

namespace PrsBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestLinesController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public RequestLinesController(PrsDbContext context)
        {
            _context = context;
        }

        // GET: api/Request-Lines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestLine()
        {
            return await _context.RequestLine.ToListAsync();
        }

        // GET: api/Request-Lines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine(int id)
        {
            var requestLine = await _context.RequestLine.FindAsync(id);

            if (requestLine == null)
            {
                return NotFound();
            }

            return requestLine;
        }

        // PUT: api/RequestLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestLine(int id, RequestLine requestLine)
        {
            if (id != requestLine.Id)
            {
                return BadRequest();
            }

            _context.Entry(requestLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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


        /* Add method to recalulaterequesttotal(requestId) - recalculates the total property whenever an 
         * insert, update, or delete occures to the Reqeustlines attached to the request.
         * method is made private so it cannot be called outsie this class.
         * it is called from PUT, Post or delete methods.
         * */

        private async Task RecalculateRequestTotal(int RequestId)
        {
            var requ = await _context.Request.FindAsync(RequestId);
            if (Request == null)
                throw new Exception("Error - Request not found to recalcualte");
            var requTotal = (from l in _context.RequestLine
                             join i in _context.Product
                             on l.ProductId equals i.Id
                             where l.RequestId == RequestId
                             select new { LineTotal = l.Quantity * i.Price })
                            .Sum(x => x.LineTotal);
            requ.Total = requTotal;
            await _context.SaveChangesAsync();

        }





        // POST: api/RequestLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine(RequestLine requestLine)
        {
            _context.RequestLine.Add(requestLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }

        // DELETE: api/RequestLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestLine(int id)
        {
            var requestLine = await _context.RequestLine.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            _context.RequestLine.Remove(requestLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestLineExists(int id)
        {
            return _context.RequestLine.Any(e => e.Id == id);
        }
    }
}
