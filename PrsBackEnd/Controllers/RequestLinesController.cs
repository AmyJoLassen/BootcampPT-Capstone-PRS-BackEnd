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

        //[Route("/totalrequests")]
        //[HttpGet]

        // SELECT @tot = SUM(rl.Quantity * p.Price)
        //  FROM Requstlines rl
        //  JOIN Products o
        //  ON rl.ProductId = p.Id
        //  WHERE rl.RequestId = 15;

        //LINQ query syntax (SQL syntax)
        //var testquery = from rl in _context.Requestlines
        //                join p in _context.Products on rl.ProductId equals p.Id
        //                where rl.RequestId == reqId
        //                select new { rl.Quantity, p.Price };
        //decimal test0 = 0;
        //foreach (var item in testquery) //SUM
        //{
        //    test0 += item.Quantity * item.Price;
        //}


        //var test1 = (
        //    from rl in _context.Requestlines
        //    join p in _context.Products on rl.ProductId equals p.Id
        //    where rl.RequestId == reqId
        //    group rl by rl.RequestId into g
        //    select g.Sum(i => 1m * i.Quantiy * i.Product.price)
        //    )
        //    .FirstOrDefault()
        //    ;

        // called a fluent API
        //var test3 = _context.RequestLine
        //    .Where(rl => rl.RequestId == reqId)
        //    .Include(rl => rl.Product)
        //    .Select(rl => new { linetotal = rl.Quantity * rl.Product.Price })
        //    .Sum(s => s.linetotal);


        // GET: api/Request-Lines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestLine()
        {
            return await _context.RequestLine.ToListAsync();
        }

        // GET: api/Request-Lines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine([FromBody] int id)
        {
            var requestLine = await _context.RequestLine.FindAsync(id);

            if (requestLine == null)
            {
                return NotFound();
            }

            return requestLine;
        }

        // PUT: api/Request-Lines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutRequestLine([FromBody] RequestLine requestLine)
        {
            //if (id != requestLine.Id)
            //{
            //    return BadRequest();
            //}

            _context.Entry(requestLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();   // update
                RecalculateRequestTotal(requestLine.RequestId);
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!RequestLineExists(id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }

            return NoContent();
        }


        /* Add method to recalulaterequesttotal(requestId) - recalculates the total property whenever an 
         * insert, update, or delete occures to the Reqeustlines attached to the request.
         * method is made private so it cannot be called outsie this class.
         * it is called from PUT, Post or delete methods.
         * */
        [Route("/totalrequests")]
        [HttpGet]
        private async Task RecalculateRequestTotal([FromBody] int RequestId)
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

            //return Ok();
        }

        // POST: api/Request-Lines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine([FromBody] RequestLine requestLine)
        {
            _context.RequestLine.Add(requestLine);
            await _context.SaveChangesAsync();
            RecalculateRequestTotal(requestLine.RequestId);

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }

        // DELETE: api/Request-Lines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestLine([FromBody] int id)
        {
            var requestLine = await _context.RequestLine.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            int theRequestId = requestLine.RequestId;

            _context.RequestLine.Remove(requestLine);
            await _context.SaveChangesAsync();

            RecalculateRequestTotal(theRequestId);

            return NoContent();
        }

        private bool RequestLineExists([FromBody] int id)
        {
            return _context.RequestLine.Any(e => e.Id == id);
        }

        // get the total
        // Find the Request
        // Update the Request
        // await _context.SaveChangesAsync();


        // ThrowExpressionSyntax new NotImplementedException();
    }
}
