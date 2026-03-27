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
    }
}