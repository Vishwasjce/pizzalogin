using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Login.Models;
using System;
using System.Threading.Tasks;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly UserContext _context;

        public OrderController(UserContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Check if the user exists
            var userExists = await _context.users.AnyAsync(u => u.UserID == order.UserId);
            if (!userExists)
            {
                return NotFound("User not found."); // Return 404 if the user doesn't exist
            }

            // Add the new order to the context
            _context.Orders.Add(order);

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Error saving order: {ex.Message}"); // Handle potential DB update issues
            }

            // Return the created order without user details
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        // Assuming you have a method to get an order by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(Guid userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found for this user.");
            }

            return Ok(orders);
        }

    }
}
