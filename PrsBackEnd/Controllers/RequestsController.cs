using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsBackEnd.Data;
using PrsBackEnd.Models;

namespace PrsBackEnd.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {

        private readonly PrsDbContext _context;

        public RequestsController(PrsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsInReview()
        {
            return await _context.Request
                .Include(x => x.User)
                .ToListAsync();
        }

        // GET: api/Requests - Gets a request in "REVIEW" status and is not owned by the used that created the request.
        [HttpGet("review/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsInReview(int userid)
        {
            return await _context.Request
                .Where(r => r.Status == PrsBackEnd.Models.Request.StatusReview && userid != r.UserId)
                .Include(x => x.User)
                .ToListAsync();
        }


        // PUT: api/Requests/5
        // Review(request) - will set the status of the request for the ID provided to 'REVIEW' 
        // unless the total of the request is >= $50. if so, set status to 'Approved'
        [HttpPut("/Review")]
        public async Task<IActionResult> PutStatusReviewOrApproved([FromBody] Request request)
        {
            var req = await _context.Request.FindAsync(request.Id);
            if (req == null)
            {
                return NotFound();
            }

            req.Status = (request.Total <= 50) ? "APPROVE" : "REVIEW";
            await _context.SaveChangesAsync();
            return Ok(request);
        }


        // PUT: (api/request/5/approve - sets the status of the request to 'Approved'
        [HttpPut("/ApproveRequest")]
        public async Task<IActionResult> Approve([FromBody] Request request)
        {

            var Requ = await _context.Request.FindAsync(request.Id);
            if (Requ == null)
            {
                return NotFound();
            }

            Requ.Status = "APPROVED";
            _context.SaveChanges();

            return Ok();
        }

        // PUT: Reject(request)  
        [HttpPut("/RejectRequest")]
        public async Task<IActionResult> Reject([FromBody] Request request)
        {
            var Requ = await _context.Request.FindAsync(request.Id);
            if (Requ == null)
            {
                return NotFound();
            }

            request.Status = "REJECTED";
            _context.SaveChanges();

            return Ok();
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> PutRequest([FromBody] Request request)
        {
            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { }


            return NoContent();
        }



        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest([FromBody] Request request)
        {
            _context.Request.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Request.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Request.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return _context.Request.Any(e => e.Id == id);
        }



    }
}
