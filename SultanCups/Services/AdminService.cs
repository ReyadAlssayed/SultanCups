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
        private const string BotToken = "8500953804:AAGll8E2_FhATfsgwRFlXuydhr_0M-uG2hA";

        // ChatId الخاص بالمطورِ}
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

            var url = $"https://api.telegram.org/bot{BotToken}/sendMessage";

            using var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("chat_id", ChatId),
        new KeyValuePair<string, string>("text", text)
    });

            var response = await _http.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        // جلب جميع المسؤولين من قاعدة البيانات
        public async Task<List<Admain>> GetAdmins()
        {
            return await _context.admins.ToListAsync();
        }
    }
}