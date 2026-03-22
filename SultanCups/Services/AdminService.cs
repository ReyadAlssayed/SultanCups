using SultanCups.Data;
using SultanCups.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace SultanCups.Services
{
    public class AdminService
    {
        private readonly AppDbContext _context;

        // عميل HTTP لإرسال الطلبات إلى Telegram API
        private readonly HttpClient _http = new HttpClient();

        // توكن البوت
        private const string BotToken = "8292571055:AAHnhwIYwEuA7_TgFgRKl7m6Q64khsc2UMY";

        // ChatId الخاص بالمطور
        private const string ChatId = "6321706551";

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        // إرسال رسالة دعم فني من النظام إلى المطور عبر بوت Telegram
        public async Task SendSupportMessage(string userName, string role, string message)
        {
            var text =
        $@"📩 رسالة دعم فني

المستخدم: {userName}
الصلاحية: {role}

الرسالة:
{message}";

            var url =
        $"https://api.telegram.org/bot{BotToken}/sendMessage?chat_id={ChatId}&text={Uri.EscapeDataString(text)}";

            await _http.GetAsync(url);
        }

        // هذا الجزء خاص بجدول admins
        // جلب جميع المسؤولين من قاعدة البيانات
        public async Task<List<Admain>> GetAdmins()
        {
            return await _context.admins.ToListAsync();
        }

        // هذا الجزء خاص بجدول admins
        // حذف مسؤول حسب admin_id
        public async Task<bool> DeleteAdmin(int adminId)
        {
            var admin = await _context.admins.FirstOrDefaultAsync(a => a.admin_id == adminId);

            if (admin == null)
                return false;

            _context.admins.Remove(admin);
            await _context.SaveChangesAsync();

            return true;
        }

        // هذا الجزء خاص بجدول admins
        // تعديل بيانات مسؤول موجود
        public async Task<bool> UpdateAdmin(Admain updatedAdmin)
        {
            var admin = await _context.admins.FirstOrDefaultAsync(a => a.admin_id == updatedAdmin.admin_id);

            if (admin == null)
                return false;

            admin.full_name = updatedAdmin.full_name;
            admin.username = updatedAdmin.username;
            admin.phone = updatedAdmin.phone;
            admin.role = updatedAdmin.role;
            admin.is_active = updatedAdmin.is_active;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}