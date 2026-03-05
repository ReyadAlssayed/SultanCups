using SultanCups.Data;
using SultanCups.Models;
using Microsoft.EntityFrameworkCore;

namespace SultanCups.Services
{
    public class AdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Admain>> GetAdmins()
        {
            return await _context.admins.ToListAsync();
        }
    }
}