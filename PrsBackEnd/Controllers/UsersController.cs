using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsBackEnd.Data;
using PrsBackEnd.Models;

namespace PrsBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public UsersController(PrsDbContext context)
        {
            _context = context;
        }



        // Incoming JSON:
        //{
        // "username": "string",
        // "password": "string"
        //}

        //[Route("login")]
        //[HttpPost]
        //public async Task<ActionResult<User>> LoginUser([FromBody] UserPasswordOdject upo)
        //{
        //    var user = await _context.Users.Where(u => u.Username == upo.Username && u.Password == upo.Password).FirstOrDefaultAsync();

        //    //var user = await (from u in_context.Users
        //    //                 where u.Username == userName && u.Password == password
        //    //                 select new { Username = u.Username, Lasstname = u.Lastname } ).FirstOrDefaultAsync();

        //    if (user == null)
        //    {
        //        return NotFound();  //404
        //    }

        //    return user;

        //}

        [Route("/login")]
        [HttpPost]
        public async Task<ActionResult<User>> LoginUser([FromBody] UserPasswordObject upo)
        {
            var user = await _context.Users.Where(u => u.Username == upo.username && u.Password == upo.password).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();  // 404
            }

            return user;  // best practice: only return what's needed!

            // anonymous type 
            //return new { Firstname = user.Firstname, Lastname = user.Lastname, Id = user.Id, IsAdmin = user.IsAdmin };

        }



        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()       // methods / ACTIONS
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }

}
