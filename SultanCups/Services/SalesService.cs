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
            return await _context.marketers.ToListAsync();
        }

        public async Task<List<Marketer>> GetActiveMarketers()
        {
            return await _context.marketers
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
            m.phone = updated.phone;
            m.address = updated.address;
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
    }
}
