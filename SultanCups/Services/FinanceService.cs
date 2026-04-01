using Microsoft.EntityFrameworkCore;
using SultanCups.Data;
using SultanCups.Models;

namespace SultanCups.Services
{
    public class FinanceService
    {
        private readonly AppDbContext _context;

        public FinanceService(AppDbContext context)
        {
            _context = context;
        }

        // هذا خاص بجدول salaries
        public async Task<List<Salary>> GetSalaries()
        {
            return await _context.salaries
                .AsNoTracking() // إضافة هذه لسرعة العرض وتوفير الذاكرة
                .Include(s => s.Employee)
                .Include(s => s.CashBox)
                .ToListAsync();
        }

        // =========================================
        // 🔹 cash_boxes (الخزن)
        // =========================================

        public async Task<List<CashBox>> GetCashBoxes()
        {
            return await _context.cash_boxes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CashBox>> GetActiveCashBoxes()
        {
            return await _context.cash_boxes
                .AsNoTracking()
                .Where(c => c.is_active)
                .ToListAsync();
        }

        public async Task AddCashBox(CashBox cashBox)
        {
            _context.cash_boxes.Add(cashBox);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateCashBox(CashBox updated)
        {
            var box = await _context.cash_boxes
                .FirstOrDefaultAsync(c => c.cash_box_id == updated.cash_box_id);

            if (box == null)
                return false;

            box.name = updated.name;
            box.is_active = updated.is_active;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleCashBox(int id)
        {
            var box = await _context.cash_boxes
                .FirstOrDefaultAsync(c => c.cash_box_id == id);

            if (box == null)
                return false;

            box.is_active = !box.is_active;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}