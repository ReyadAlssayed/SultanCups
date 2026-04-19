using SultanCups.Data;
using SultanCups.Models;
using Microsoft.EntityFrameworkCore;


namespace SultanCups.Services
{
    public class SalesService
    {
        private readonly AppDbContext _context;

        public SalesService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================
        // 🔹 marketers (المسوقين)
        // =========================================

        // جلب جميع المسوقين
        public async Task<List<Marketer>> GetMarketers()
        {
            return await _context.marketers
    .AsNoTracking()
    .ToListAsync();
        }

        public async Task<List<Marketer>> GetActiveMarketers()
        {
            return await _context.marketers
     .AsNoTracking()
     .Where(m => m.is_active)
     .ToListAsync();
        }

        // إضافة مسوق
        public async Task AddMarketer(Marketer marketer)
        {
            marketer.name = marketer.name.Trim();
            marketer.notes = marketer.notes?.Trim();

            _context.marketers.Add(marketer);
            await _context.SaveChangesAsync();
        }

        // تعديل مسوق
        public async Task<bool> UpdateMarketer(Marketer updated)
        {
            var m = await _context.marketers
                .FirstOrDefaultAsync(x => x.marketer_id == updated.marketer_id);

            if (m == null)
                return false;

            m.name = updated.name.Trim();
            m.phone = updated.phone?.Trim();
            m.address = updated.address?.Trim();
            m.commission_per_box = updated.commission_per_box;
            m.marketer_type = updated.marketer_type;
            m.notes = updated.notes?.Trim();
            m.is_active = updated.is_active;

            await _context.SaveChangesAsync();
            return true;
        }

        // حذف مسوق (مسموح حالياً لأنه غير مربوط مباشرة)
        // لو ربطته لاحقاً → حوله لتعطيل
        public async Task<bool> ToggleMarketer(int id)
        {
            var m = await _context.marketers
                .FirstOrDefaultAsync(x => x.marketer_id == id);

            if (m == null)
                return false;

            m.is_active = !m.is_active;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> DeleteOrDisableMarketer(int id)
        {
            var m = await _context.marketers
                .FirstOrDefaultAsync(x => x.marketer_id == id);

            if (m == null)
                return "not_found";

            var hasOrders = await _context.orders
                .AnyAsync(o => o.marketer_id == id);

            if (hasOrders)
            {
                m.is_active = false;
                await _context.SaveChangesAsync();
                return "disabled";
            }

            _context.marketers.Remove(m);
            await _context.SaveChangesAsync();
            return "deleted";
        }


        // =========================================
        // 🔹 customers (الزبائن)
        // =========================================

        // جلب جميع الزبائن
        public async Task<List<Customer>> GetCustomers()
        {
            return await _context.customers
                .AsNoTracking()
                .ToListAsync();
        }

        // جلب الزبائن النشطين
        public async Task<List<Customer>> GetActiveCustomers()
        {
            return await _context.customers
                .AsNoTracking()
                .Where(c => c.is_active)
                .ToListAsync();
        }

        // إضافة زبون
        public async Task AddCustomer(Customer customer)
        {
            customer.name = customer.name.Trim();
            customer.notes = customer.notes?.Trim();

            _context.customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        // تعديل زبون
        public async Task<bool> UpdateCustomer(Customer updated)
        {
            var c = await _context.customers
                .FirstOrDefaultAsync(x => x.customer_id == updated.customer_id);

            if (c == null)
                return false;

            c.name = updated.name.Trim();
            c.phone = updated.phone?.Trim();
            c.address = updated.address?.Trim();
            c.notes = updated.notes?.Trim();
            c.is_active = updated.is_active;

            await _context.SaveChangesAsync();
            return true;
        }



        // حذف أو تعطيل زبون
        public async Task<string> DeleteOrDisableCustomer(int id)
        {
            var c = await _context.customers
                .FirstOrDefaultAsync(x => x.customer_id == id);

            if (c == null)
                return "not_found";

            var hasOrders = await _context.orders
                .AnyAsync(o => o.customer_id == id);

            if (hasOrders)
            {
                c.is_active = false;
                await _context.SaveChangesAsync();
                return "disabled";
            }

            _context.customers.Remove(c);
            await _context.SaveChangesAsync();
            return "deleted";
        }

        // =========================================
        // 🔹 other_purchases (المصروفات الأخرى)
        // =========================================

        // جلب الكل
        public async Task<List<OtherPurchase>> GetOtherPurchases()
        {
            return await _context.other_purchases
                .AsNoTracking()
                .ToListAsync();
        }

        // إضافة
        public async Task AddOtherPurchase(OtherPurchase item)
        {
            item.name = item.name.Trim();
            item.notes = item.notes?.Trim();

            _context.other_purchases.Add(item);
            await _context.SaveChangesAsync();
        }

        // تعديل
        public async Task<bool> UpdateOtherPurchase(OtherPurchase updated)
        {
            var p = await _context.other_purchases
                .FirstOrDefaultAsync(x => x.other_purchase_id == updated.other_purchase_id);

            if (p == null)
                return false;

            p.name = updated.name.Trim();
            p.quantity = updated.quantity;
            p.cost = updated.cost;
            p.purchase_date = updated.purchase_date;
            p.notes = updated.notes?.Trim();
            p.cash_box_id = updated.cash_box_id;

            await _context.SaveChangesAsync();
            return true;
        }

        // حذف مباشر
        public async Task<bool> DeleteOtherPurchase(int id)
        {
            var p = await _context.other_purchases
                .FirstOrDefaultAsync(x => x.other_purchase_id == id);

            if (p == null)
                return false;

            _context.other_purchases.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
