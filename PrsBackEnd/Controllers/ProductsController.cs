﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrsBackEnd.Data;
using PrsBackEnd.Models;

namespace PrsBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly PrsDbContext _context;

        public ProductsController(PrsDbContext context)
        {
            _context = context;
        }

        // GET: api/Products            (Get all Products)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Product.Include(p => p.Vendor).ToListAsync();
        }


        // GET: api/Products/5          (Get Products by specific Id#)
        [HttpGet("/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Product
                .Include(v => v.Vendor)
                .SingleOrDefaultAsync(product => product.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }


        // PUT: api/Products/Id
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
