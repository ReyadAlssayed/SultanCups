using SultanCups.Data;
using SultanCups.Models;
using Microsoft.EntityFrameworkCore;

namespace SultanCups.Services
{
    public class HrService
    {
        private readonly AppDbContext _context;

        public HrService(AppDbContext context)
        {
            _context = context;
        }

        // هذا الجزء خاص بجدول employees
        // جلب جميع الموظفين من قاعدة البيانات
        public async Task<List<Employee>> GetEmployees()
        {
            return await _context.employees
                .AsNoTracking() // إضافة هذه للسرعة
                .ToListAsync();
        }


        //هدا خاص بالبحث عن موظف
        public async Task<List<Employee>> SearchEmployees(string? searchText, bool? isActive = null)
        {
            // أضفنا AsNoTracking هنا في بداية الاستعلام
            var query = _context.employees.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();
                query = query.Where(e =>
                    e.full_name.Contains(searchText) ||
                    (e.phone != null && e.phone.Contains(searchText)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(e => e.is_active == isActive.Value);
            }

            return await query.ToListAsync();
        }

        // هذا الجزء خاص بجدول employees
        // إضافة موظف جديد
        public async Task AddEmployee(Employee employee)
        {
            _context.employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        // هذا الجزء خاص بجدول employees
        // تعديل بيانات موظف موجود حسب employee_id
        public async Task<bool> UpdateEmployee(Employee updatedEmployee)
        {
            var employee = await _context.employees
                .FirstOrDefaultAsync(e => e.employee_id == updatedEmployee.employee_id);

            if (employee == null)
                return false;

            employee.full_name = updatedEmployee.full_name;
            employee.phone = updatedEmployee.phone;
            employee.rank = updatedEmployee.rank;
            employee.base_salary = updatedEmployee.base_salary;
            employee.is_active = updatedEmployee.is_active;

            await _context.SaveChangesAsync();
            return true;
        }

        // هذا الجزء خاص بجدول employees
        // حذف الموظف إذا لم يكن مرتبطًا بسجلات رواتب
        // وإذا كان مرتبطًا برواتب يتم تعطيله بدل الحذف
        public async Task<string> DeleteOrDisableEmployee(int employeeId)
        {
            var employee = await _context.employees
                .FirstOrDefaultAsync(e => e.employee_id == employeeId);

            if (employee == null)
                return "not_found";

            var hasSalaries = await _context.salaries
                .AnyAsync(s => s.employee_id == employeeId);

            if (hasSalaries)
            {
                employee.is_active = false;
                await _context.SaveChangesAsync();
                return "disabled";
            }

            _context.employees.Remove(employee);
            await _context.SaveChangesAsync();
            return "deleted";
        }


    }
}