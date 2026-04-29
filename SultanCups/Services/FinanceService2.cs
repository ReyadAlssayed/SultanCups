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
        public async Task<(bool success, string message)> AddOrder(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🔍 1. جلب كل المخزون مرة واحدة
                var productIds = order.Items.Select(i => i.product_id).ToList();

                var stocks = await _context.product_stock
                    .Where(s => productIds.Contains(s.product_id))
                    .ToDictionaryAsync(s => s.product_id);

                // 🔍 2. تحقق من المخزون
                foreach (var item in order.Items)
                {
                    if (!stocks.ContainsKey(item.product_id))
                        return (false, $"المنتج غير موجود (ID={item.product_id})");

                    if (stocks[item.product_id].quantity < item.quantity)
                        return (false, $"المخزون غير كافي (المتوفر={stocks[item.product_id].quantity})");
                }

                // 🧾 3. حفظ الفاتورة
                _context.orders.Add(order);
                await _context.SaveChangesAsync();

                // 📦 4. التفاصيل + خصم المخزون
                foreach (var item in order.Items)
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
    }
}