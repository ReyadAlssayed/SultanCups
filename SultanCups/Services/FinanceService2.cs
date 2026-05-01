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

        private void AddFinancialEvent(
    string type,
    string direction,
    decimal amount,
    int cashBoxId,
    int adminId,
    int refId,
    string refTable,
    int? personId,
    string? personName,
    string notes = "",
    int? itemId = null,
    string? itemName = null)
        {
            var adminName = _context.admins
                .Where(x => x.admin_id == adminId)
                .Select(x => x.full_name)
                .FirstOrDefault();

            _context.financial_events.Add(new FinancialEvent
            {
                event_type = type,
                direction = direction,
                amount = amount,
                cash_box_id = cashBoxId,

                performed_by = adminId,
                admin_name_snapshot = adminName,

                ref_table = refTable,
                ref_id = refId,

                person_id = personId,
                person_name_snapshot = personName,

                item_id = itemId,
                item_name_snapshot = itemName,

                event_date = DateTime.UtcNow,
                notes = notes
            });
        }

        // ✅ إنشاء فاتورة
        public async Task<(bool success, string message)> AddOrder(
      Order order,
      List<OrderItem> items,
      int adminId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                order.Items = new List<OrderItem>();

                var productIds = items.Select(i => i.product_id).ToList();

                var stocks = await _context.product_stock
                    .Where(s => productIds.Contains(s.product_id))
                    .ToDictionaryAsync(s => s.product_id);

                // تحقق المخزون
                foreach (var item in items)
                {
                    if (!stocks.ContainsKey(item.product_id))
                        return (false, $"المنتج غير موجود (ID={item.product_id})");

                    if (stocks[item.product_id].quantity < item.quantity)
                        return (false, $"المخزون غير كافي (المتوفر={stocks[item.product_id].quantity})");
                }

                // حفظ الفاتورة
                _context.orders.Add(order);
                await _context.SaveChangesAsync();

                // حفظ الأصناف + خصم المخزون
                foreach (var item in items)
                {
                    item.order_id = order.order_id;

                    _context.order_items.Add(item);

                    stocks[item.product_id].quantity -= item.quantity;
                }

                await _context.SaveChangesAsync();

                // =========================================
                // 🔥 جلب اسم الشخص
                // =========================================
                string personName = "";

                if (order.person_type == "customer")
                {
                    personName = await _context.customers
                        .Where(c => c.customer_id == order.person_id)
                        .Select(c => c.name)
                        .FirstOrDefaultAsync();
                }
                else if (order.person_type == "marketer")
                {
                    personName = await _context.marketers
                        .Where(m => m.marketer_id == order.person_id)
                        .Select(m => m.name)
                        .FirstOrDefaultAsync();
                }

                // 🔥 هنا
                personName = order.person_type == "customer"
                    ? $"{personName} (زبون)"
                    : $"{personName} (مسوق)";

                // =========================================
                // 🔥 جلب أول منتج
                // =========================================
                var firstProduct = await _context.products
                    .Where(p => p.product_id == items.First().product_id)
                    .Select(p => new { p.product_id, p.name })
                    .FirstOrDefaultAsync();

                // =========================================
                // 🔥 تسجيل الحركة المالية
                // =========================================
                if (order.payment_method != "debt" && order.paid_amount > 0)
                {
                    AddFinancialEvent(
                        "فاتورة بيع جديدة",
                        "IN",
                        order.paid_amount,
                        order.cash_box_id,
                        adminId,
                        order.order_id,
                        "orders",
                        order.person_id,
                        personName,
                        "تحصيل فاتورة",
                        firstProduct?.product_id,
                        firstProduct != null ? $"{firstProduct.name} (منتج)" : null
                    );
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, order.order_id.ToString());
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
    .Sum(i => (decimal?)(i.quantity * i.unit_price)) ?? 0,

                    net_total =
    ((_context.order_items
        .Where(i => i.order_id == o.order_id)
        .Sum(i => (decimal?)(i.quantity * i.unit_price)) ?? 0)
    - o.discount_total),

                    commission_total = o.person_type == "marketer"
                        ? ((_context.order_items
                            .Where(i => i.order_id == o.order_id)
                            .Sum(i => (int?)i.quantity) ?? 0)
                            * o.commission_per_box)
                        : 0,

                    paid_amount = o.paid_amount,   // 🔥 هذا الناقص
                    order_date = o.order_date
                };

            return await query
                .OrderByDescending(o => o.order_id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        //جلب الخزنات النشطة
        public async Task<List<CashBox>> GetCashBoxes()
        {
            return await _context.cash_boxes
                .Where(c => c.is_active)
                .ToListAsync();
        }


        //جلب معلومات الفاتورة برقم القيد للفاتورة

        public async Task<Order?> GetOrderById(int id)
        {
            return await _context.orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.order_id == id);
        }

    }
}