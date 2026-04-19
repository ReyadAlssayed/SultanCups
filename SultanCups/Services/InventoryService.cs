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

        public async Task<List<Product>> GetProducts()
        {
            return await _context.products
                .AsNoTracking() // الإضافة هنا
                .Where(p => p.is_active)
                .ToListAsync();
        }

        public async Task AddProduct(Product product)
        {
            _context.products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.products.ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product updatedProduct)
        {
            var product = await _context.products
                .FirstOrDefaultAsync(p => p.product_id == updatedProduct.product_id);

            if (product == null)
                return false;

            product.name = updatedProduct.name;
            // السطر الناقص الذي يجب إضافته:
            product.is_active = updatedProduct.is_active;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var product = await _context.products
                .FirstOrDefaultAsync(p => p.product_id == productId);

            if (product == null)
                return false;

            product.is_active = false;

            await _context.SaveChangesAsync();
            return true;
        }

        // =========================================
        // 🔹 هذا الجزء خاص بجدول production (الإنتاج)
        // =========================================

        public async Task<List<Production>> GetProduction()
        {
            return await _context.production
                .AsNoTracking() // الإضافة هنا
                .Include(p => p.Product)
                .ToListAsync();
        }

        public async Task AddProduction(Production production)
        {
            production.production_date = DateTime.SpecifyKind(
                production.production_date,
                DateTimeKind.Unspecified);

            production.notes = production.notes?.Trim();

            _context.production.Add(production);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateProduction(Production updated)
        {
            var prod = await _context.production
                .FirstOrDefaultAsync(p => p.production_id == updated.production_id);

            if (prod == null)
                return false;

            prod.product_id = updated.product_id;
            prod.box_cost = updated.box_cost;
            prod.box_count = updated.box_count;
            prod.notes = updated.notes?.Trim(); 

            prod.production_date = DateTime.SpecifyKind(
                updated.production_date,
                DateTimeKind.Unspecified);

            await _context.SaveChangesAsync();
            return true;
        }

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

        // =========================================
        // 🔹 هذا الجزء خاص بجدول suppliers (الموردين)
        // =========================================

        // جلب جميع الموردين
        public async Task<List<Supplier>> GetSuppliers()
        {
            return await _context.suppliers
                .AsNoTracking()
                .ToListAsync();
        }

        // إضافة مورد جديد
        public async Task AddSupplier(Supplier supplier)
        {
            _context.suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        // تعديل مورد
        public async Task<bool> UpdateSupplier(Supplier updated)
        {
            var supplier = await _context.suppliers
                .FirstOrDefaultAsync(s => s.supplier_id == updated.supplier_id);

            // لو غير موجود
            if (supplier == null)
                return false;

            // تحديث البيانات
            supplier.name = updated.name;
            supplier.phone = updated.phone;
            supplier.email = updated.email;
            supplier.location = updated.location;
            supplier.is_active = updated.is_active;
            supplier.notes = updated.notes;

            await _context.SaveChangesAsync();
            return true;
        }

        // حذف مورد
        public async Task<string> DeleteOrDisableSupplier(int id)
        {
            var supplier = await _context.suppliers
                .FirstOrDefaultAsync(s => s.supplier_id == id);

            if (supplier == null)
                return "not_found";

            // ✅ الربط الصحيح هنا
            var hasPurchases = await _context.purchases
                .AnyAsync(p => p.supplier_id == id);

            if (hasPurchases)
            {
                supplier.is_active = false;
                await _context.SaveChangesAsync();
                return "disabled";
            }

            _context.suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return "deleted";
        }

        public async Task<List<Supplier>> GetAllSuppliers()
        {
            return await _context.suppliers
                .AsNoTracking()
                .ToListAsync();
        }

        // =========================================
        // 🔹 raw_materials (المواد الخام)
        // =========================================

        public async Task<List<RawMaterial>> GetActiveRawMaterials()
        {
            return await _context.raw_materials
                .AsNoTracking()
                .Where(r => r.is_active)
                .ToListAsync();
        }

        public async Task<List<RawMaterial>> GetAllRawMaterials()
        {
            return await _context.raw_materials
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddRawMaterial(RawMaterial material)
        {
            _context.raw_materials.Add(material);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateRawMaterial(RawMaterial updated)
        {
            var material = await _context.raw_materials
                .FirstOrDefaultAsync(r => r.raw_material_id == updated.raw_material_id);

            if (material == null)
                return false;

            material.name = updated.name;
            material.size = updated.size;
            material.unit_of_measure = updated.unit_of_measure;
            material.unit_cost = updated.unit_cost;
            material.is_active = updated.is_active;
            material.notes = updated.notes;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleRawMaterial(int id)
        {
            var material = await _context.raw_materials
                .FirstOrDefaultAsync(r => r.raw_material_id == id);

            if (material == null)
                return false;

            material.is_active = !material.is_active;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}