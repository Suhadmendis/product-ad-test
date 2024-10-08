using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test001.Data;
using test001.Models;
using System.Linq;
using System.Threading.Tasks;

namespace test001.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] Product product)
        {
            // Validate the product object
            if (product == null || string.IsNullOrEmpty(product.Name) || product.Price <= 0)
            {
                return BadRequest("Invalid product data.");
            }

            // Add the new product to the database
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Return a success response
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product updatedProduct)
        {
            // Validate the updated product object
            if (updatedProduct == null || string.IsNullOrEmpty(updatedProduct.Name) || updatedProduct.Price <= 0)
            {
                return BadRequest("Invalid product data.");
            }

            // Find the product by ID
            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound(new { message = $"Product with id {id} not found." });
            }

            // Update the existing product with the new data
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success response
            return Ok(existingProduct);
        }


        [HttpGet("search")]
        public async Task<IActionResult> Search(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Search term cannot be empty.");
            }

            // Search products by name (case-insensitive)
            var products = await _context.Products
                .Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            // If no products are found
            if (!products.Any())
            {
                return NotFound(new { message = $"No products found with the name '{name}'." });
            }

            // Return the list of products
            return Ok(products);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = $"Product with id {id} not found." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Product with id {id} has been deleted." });
        }


    }
}