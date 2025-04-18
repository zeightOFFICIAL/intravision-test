using Microsoft.EntityFrameworkCore;
using server.Context;
using server.Models;

namespace server.Repositories
{
    public class OrderRepository(
        VendingMachineContext context,
        ILogger<OrderRepository> logger) : IOrderRepository
    {
        private readonly VendingMachineContext _context = context;
        private readonly ILogger<OrderRepository> _logger = logger;

        public async Task AddOrderAsync(Order order)
        {
            try
            {
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding order");
                throw;
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            try
            {
                _context.Entry(order).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await OrderExistsAsync(order.Id))
                {
                    _logger.LogError("Order not found during update: {OrderId}", order.Id);
                    throw new KeyNotFoundException($"Order with ID {order.Id} not found");
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating order: {OrderId}", order.Id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order: {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order by ID: {OrderId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        private async Task<bool> OrderExistsAsync(int id)
        {
            return await _context.Orders.AnyAsync(e => e.Id == id);
        }
    }
}