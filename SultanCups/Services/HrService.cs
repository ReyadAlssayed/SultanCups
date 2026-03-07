using Microsoft.EntityFrameworkCore;
using SultanCups.Data;
using SultanCups.Models;

namespace SultanCups.Services
{
    public class HrService
    {
        private readonly AppDbContext _context;

        public HrService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetEmployees()
        {
            return await _context.employees.ToListAsync();
        }
    }
}