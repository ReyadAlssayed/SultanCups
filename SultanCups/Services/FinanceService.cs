using Microsoft.EntityFrameworkCore;
using SultanCups.Data;
using SultanCups.Models;
using System.Text.Json;

namespace SultanCups.Services
{
    public class FinanceService
    {
        private readonly AppDbContext _context;


    public FinanceService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================
        // 🔹 Helpers (مشتركة)
        // =========================================

        private void AddFinancialEvent(
            string type,
            string direction,
            decimal amount,
            int cashBoxId,
            int adminId,
            int refId,
            int? employeeId,
            string? employeeName,
            string notes)
        {
            // 🔥 العمليات التي تحتاج موظف
            var employeeEvents = new[]
            {
        "دفع راتب",
        "استرجاع راتب",
        "دفع عمولة",
        "رصيد لصالح المسوق",
        "استخدام رصيد"
    };

            // 🔥 تحقق
            if (employeeEvents.Contains(type))
            {
                if (employeeId == null || string.IsNullOrWhiteSpace(employeeName))
                    throw new Exception("هذا النوع من العمليات يتطلب موظف");
            }

            _context.financial_events.Add(new FinancialEvent
            {
                event_type = type,
                direction = direction,
                amount = amount,
                cash_box_id = cashBoxId,
                performed_by = adminId,
                ref_table = "salaries",
                ref_id = refId,
                employee_id = employeeId,
                person_name_snapshot = employeeName,
                event_date = DateTime.UtcNow,
                notes = notes
            });
        }
        private void AddAudit(
            string table,
            string operation,
            string recordId,
            object? oldData,
            object? newData,
            int adminId)
        {
            _context.audit_log.Add(new AuditLog
            {
                table_name = table,
                operation = operation,
                record_id = recordId,
                old_data = oldData != null ? JsonSerializer.Serialize(oldData) : null,
                new_data = newData != null ? JsonSerializer.Serialize(newData) : null,
                performed_by = adminId,
                performed_at = DateTime.UtcNow
            });
        }

        private void UpdateSalaryStatus(Salary salary)
        {
            if (salary.paid_amount == 0)
                salary.status = "غير خالص";
            else if (salary.paid_amount < salary.amount)
                salary.status = "خالص جزئي";
            else
                salary.status = "خالص";
        }

        // =========================================
        // 🔹 Employees
        // =========================================

        public async Task<List<Employee>> GetActiveEmployees()
        {
            return await _context.employees
                .Where(e => e.is_active)
                .ToListAsync();
        }

        // =========================================
        // 🔹 Salaries
        // =========================================

        public async Task<List<Salary>> GetSalaries()
        {
            return await _context.salaries
                .AsNoTracking()
                .Include(s => s.Employee)
                .Include(s => s.CashBox)
                .ToListAsync();
        }

        public async Task AddSalary(Salary salary, int adminId)
        {
            if (string.IsNullOrWhiteSpace(salary.salary_type))
                salary.salary_type = "راتب أساسي";

            if (salary.paid_amount <= 0)
                throw new Exception("يجب إدخال مبلغ مدفوع");

            var existing = await _context.salaries
                .FirstOrDefaultAsync(s =>
                    s.employee_id == salary.employee_id &&
                    s.salary_date.Year == salary.salary_date.Year &&
                    s.salary_date.Month == salary.salary_date.Month);

            if (existing != null)
                throw new Exception("تم صرف راتب لهذا الموظف خلال هذا الشهر، استخدم إضافة دفعة إذا كان هناك مبلغ متبقٍ");

            UpdateSalaryStatus(salary);

            salary.salary_date = DateTime.SpecifyKind(
                salary.salary_date,
                DateTimeKind.Utc);

            var balance = await GetBalanceFromView(salary.cash_box_id);

            if (salary.paid_amount > balance)
                throw new Exception("رصيد الخزنة غير كافٍ");

            _context.salaries.Add(salary);
            await _context.SaveChangesAsync();

            var employee = await _context.employees
                .AsNoTracking()
                .FirstAsync(e => e.employee_id == salary.employee_id);

            AddFinancialEvent(
                "دفع راتب",
                "OUT",
                salary.paid_amount,
                salary.cash_box_id,
                adminId,
                salary.salary_id,
                salary.employee_id,
                employee.full_name,
                "دفع راتب عند الإنشاء"
            );

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddSalaryPayment(int salaryId, decimal amount, int adminId)
        {
            var salary = await _context.salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.salary_id == salaryId);

            if (salary == null) return false;

            var remaining = salary.amount - salary.paid_amount;

            var balance = await GetBalanceFromView(salary.cash_box_id);
            if (amount > balance)
                return false;

            if (amount <= 0 || amount > remaining)
                return false;

            salary.paid_amount += amount;

            UpdateSalaryStatus(salary);

            AddFinancialEvent(
                "دفع راتب",
                "OUT",
                amount,
                salary.cash_box_id,
                adminId,
                salary.salary_id,
                salary.employee_id,
                salary.Employee.full_name,
                "إضافة دفعة راتب"
            );

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReverseSalary(int salaryId, decimal amount, int adminId)
        {
            var salary = await _context.salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.salary_id == salaryId);

            if (salary == null) return false;

            if (amount <= 0 || amount > salary.paid_amount)
                return false;

            salary.paid_amount -= amount;

            UpdateSalaryStatus(salary);

            AddFinancialEvent(
                "استرجاع راتب",
                "IN",
                amount,
                salary.cash_box_id,
                adminId,
                salary.salary_id,
                salary.employee_id,
                salary.Employee.full_name,
                "استرجاع راتب"
            );

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateSalary(Salary updated, int adminId)
        {
            var salary = await _context.salaries
                .FirstOrDefaultAsync(x => x.salary_id == updated.salary_id);

            if (salary == null) return;

            // 🔥 منع تكرار راتب لنفس الموظف في نفس الشهر عند تغيير الموظف
            if (salary.employee_id != updated.employee_id)
            {
                var year = salary.salary_date.Year;
                var month = salary.salary_date.Month;

                var exists = await _context.salaries
                    .AnyAsync(s =>
                        s.employee_id == updated.employee_id &&
                        s.salary_id != salary.salary_id &&
                        s.salary_date.Year == year &&
                        s.salary_date.Month == month);

                if (exists)
                    throw new Exception("لا يمكن إضافة راتب لهذا الموظف، لديه راتب أساسي في نفس الشهر");
            }

            var oldData = new Dictionary<string, object>();
            var newData = new Dictionary<string, object>();

            if (salary.employee_id != updated.employee_id)
            {
                var oldEmp = await _context.employees.FindAsync(salary.employee_id);
                var newEmp = await _context.employees.FindAsync(updated.employee_id);

                oldData["employee"] = oldEmp?.full_name ?? "";
                newData["employee"] = newEmp?.full_name ?? "";
            }

            if (salary.amount != updated.amount)
            {
                oldData["amount"] = salary.amount;
                newData["amount"] = updated.amount;
            }

            if ((salary.notes ?? "") != (updated.notes ?? ""))
            {
                oldData["notes"] = salary.notes ?? "";
                newData["notes"] = updated.notes ?? "";
            }

            if (oldData.Count > 0)
            {
                AddAudit(
                    "salaries",
                    "UPDATE",
                    salary.salary_id.ToString(),
                    oldData,
                    newData,
                    adminId
                );
            }

            // 🔥 نقل الراتب بين الخزن
            if (salary.cash_box_id != updated.cash_box_id && salary.paid_amount > 0)
            {
                var balance = await GetBalanceFromView(updated.cash_box_id);

                if (salary.paid_amount > balance)
                    throw new Exception("رصيد الخزنة الجديدة غير كافي");

                var employee = await _context.employees
                    .FirstOrDefaultAsync(e => e.employee_id == salary.employee_id);

                AddFinancialEvent(
                    "استرجاع راتب",
                    "IN",
                    salary.paid_amount,
                    salary.cash_box_id,
                    adminId,
                    salary.salary_id,
                    salary.employee_id,
                    employee?.full_name,
                    "نقل الراتب من خزنة قديمة"
                );

                AddFinancialEvent(
                    "دفع راتب",
                    "OUT",
                    salary.paid_amount,
                    updated.cash_box_id,
                    adminId,
                    salary.salary_id,
                    salary.employee_id,
                    employee?.full_name,
                    "نقل الراتب إلى خزنة جديدة"
                );
            }

            salary.amount = updated.amount;
            salary.salary_type = updated.salary_type;
            salary.cash_box_id = updated.cash_box_id;
            salary.notes = updated.notes;
            salary.employee_id = updated.employee_id;
            salary.salary_date = DateTime.SpecifyKind(updated.salary_date, DateTimeKind.Utc);

            UpdateSalaryStatus(salary);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteSalary(int salaryId, int adminId)
        {
            var salary = await _context.salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.salary_id == salaryId);

            if (salary == null) return false;

            if (salary.paid_amount > 0)
            {
                AddFinancialEvent(
                    "استرجاع راتب",
                    "IN",
                    salary.paid_amount,
                    salary.cash_box_id,
                    adminId,
                    salary.salary_id,
                    salary.employee_id,
                    salary.Employee.full_name,
                    "حذف راتب - استرجاع كامل"
                );
            }

            var data = new
            {
                employee = salary.Employee.full_name,
                amount = salary.amount,
                paid = salary.paid_amount,
                notes = salary.notes
            };

            AddAudit(
                "salaries",
                "DELETE",
                salary.salary_id.ToString(),
                data,
                null,
                adminId
            );

            _context.salaries.Remove(salary);

            await _context.SaveChangesAsync();

            return true;
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
        public async Task<List<CashBoxBalance>> GetCashBoxBalances()
        {
            return await _context.cash_box_balances.ToListAsync();
        }
        public async Task<decimal> GetBalanceFromView(int cashBoxId)
        {
            return await _context.cash_box_balances
                .Where(x => x.cash_box_id == cashBoxId)
                .Select(x => x.balance)
                .FirstOrDefaultAsync();
        }

        //جلب السجلات المالية كاملة

        public async Task<List<FinancialEvent>> GetFinancialEvents()
        {
            return await _context.financial_events
                .AsNoTracking()
                .Include(x => x.CashBox)
                .Include(x => x.Admin)
                .OrderByDescending(x => x.event_date)
                .ToListAsync();
        }
    }

}
