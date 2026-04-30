using Microsoft.EntityFrameworkCore;
using SultanCups.Data;
using SultanCups.Models;

namespace SultanCups.Services
{
    public class FinanceService2
    {
        private readonly AppDbContext _context;

        public FinanceService2(AppDbContext context)
        {
            _context = context;
        }

        // ✅ إنشاء فاتورة
        public async Task<(bool success, string message)> AddOrder(Order order, List<OrderItem> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🔥 مهم
                order.Items = new List<OrderItem>();

                var productIds = items.Select(i => i.product_id).ToList();

                var stocks = await _context.product_stock
                    .Where(s => productIds.Contains(s.product_id))
                    .ToDictionaryAsync(s => s.product_id);

                foreach (var item in items)
                {
                    if (!stocks.ContainsKey(item.product_id))
                        return (false, $"المنتج غير موجود (ID={item.product_id})");

                    if (stocks[item.product_id].quantity < item.quantity)
                        return (false, $"المخزون غير كافي (المتوفر={stocks[item.product_id].quantity})");
                }

                _context.orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    item.order_id = order.order_id;
                    item.total = item.quantity * item.unit_price;

                    _context.order_items.Add(item);

                    stocks[item.product_id].quantity -= item.quantity;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "تم حفظ الفاتورة ✔");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                var msg = ex.InnerException?.Message ?? ex.Message;
                return (false, msg);
            }
        }

        public async Task<List<OrderView>> GetOrders(int page = 1, int pageSize = 20)
        {
            var query =
                from o in _context.orders
                join c in _context.customers on o.person_id equals c.customer_id into cg
                from c in cg.DefaultIfEmpty()
                join m in _context.marketers on o.person_id equals m.marketer_id into mg
                from m in mg.DefaultIfEmpty()
                select new OrderView
                {
                    order_id = o.order_id,
                    person_type = o.person_type,
                    discount_total = o.discount_total,
                    person_name = o.person_type == "customer" ? c.name : m.name,

                    items_count = _context.order_items.Count(i => i.order_id == o.order_id),

                    total = _context.order_items
                        .Where(i => i.order_id == o.order_id)
                        .Sum(i => (decimal?)i.total) ?? 0,

                    net_total =
                        ((_context.order_items
                            .Where(i => i.order_id == o.order_id)
                            .Sum(i => (decimal?)i.total) ?? 0)
                        - o.discount_total),

                    commission_total = o.person_type == "marketer"
                        ? ((_context.order_items
                            .Where(i => i.order_id == o.order_id)
                            .Sum(i => (int?)i.quantity) ?? 0)
                            * o.commission_per_box)
                        : 0,

                    order_date = o.order_date
                };

            return await query
                .OrderByDescending(o => o.order_id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}