using SultanCups.Data;
using SultanCups.Models;
using Microsoft.EntityFrameworkCore;

namespace SultanCups.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        // ================================
        // 🔹 هذا الجزء خاص بجدول products
        // ================================

        // جلب جميع المنتجات من قاعدة البيانات
        public async Task<List<Product>> GetProducts()
        {
            return await _context.products.ToListAsync();
        }

        // إضافة منتج جديد
        public async Task AddProduct(Product product)
        {
            _context.products.Add(product);
            await _context.SaveChangesAsync();
        }

        // تعديل منتج موجود حسب product_id
        public async Task<bool> UpdateProduct(Product updatedProduct)
        {
            var product = await _context.products
                .FirstOrDefaultAsync(p => p.product_id == updatedProduct.product_id);

            if (product == null)
                return false;

            product.name = updatedProduct.name;

            await _context.SaveChangesAsync();
            return true;
        }

        // حذف منتج حسب product_id
        public async Task<bool> DeleteProduct(int productId)
        {
            var product = await _context.products
                .FirstOrDefaultAsync(p => p.product_id == productId);

            if (product == null)
                return false;

            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================
        // 🔹 هذا الجزء خاص بجدول production (الإنتاج)
        // =========================================

        // جلب جميع عمليات الإنتاج مع اسم المنتج
        public async Task<List<Production>> GetProduction()
        {
            return await _context.production
                .Include(p => p.Product) // ربط مع جدول المنتجات لجلب الاسم
                .ToListAsync();
        }

        // إضافة عملية إنتاج جديدة
        // إضافة عملية إنتاج جديدة
        public async Task AddProduction(Production production)
        {
            production.production_date = DateTime.SpecifyKind(production.production_date, DateTimeKind.Unspecified);

            _context.production.Add(production);
            await _context.SaveChangesAsync();
        }

        // تعديل عملية إنتاج حسب production_id
        public async Task<bool> UpdateProduction(Production updated)
        {
            var prod = await _context.production
                .FirstOrDefaultAsync(p => p.production_id == updated.production_id);

            if (prod == null)
                return false;

            prod.product_id = updated.product_id;
            prod.box_cost = updated.box_cost;
            prod.box_count = updated.box_count;
            prod.production_date = DateTime.SpecifyKind(updated.production_date, DateTimeKind.Unspecified);

            await _context.SaveChangesAsync();
            return true;
        }

        // حذف عملية إنتاج
        public async Task<bool> DeleteProduction(int id)
        {
            var prod = await _context.production
                .FirstOrDefaultAsync(p => p.production_id == id);

            if (prod == null)
                return false;

            _context.production.Remove(prod);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}