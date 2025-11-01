
using System.Globalization;
using System.Runtime.CompilerServices;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //Last month
            DateTime StartDate = DateTime.Today.AddDays(-30);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
            .Include(x => x.Category).Where(y => y.Date >= StartDate && y.Date <= EndDate)
            .ToListAsync();

            //Total Income
            int TotalIncome = SelectedTransactions
            .Where(i => i.Category.Type == "Income")
            .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //Total Expense
            int TotalExpense = SelectedTransactions
            .Where(i => i.Category.Type == "Expense")
            .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //Balance
            int Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C0");

            //Doughnut Chart - Expense By Category
            ViewBag.DoughnutChartData = SelectedTransactions
            .Where(i => i.Category.Type == "Expense").GroupBy(j => j.CategoryId)
            .Select(k => new
            {
                CategoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                amount = k.Sum(j => j.Amount),
                formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
            }).OrderByDescending(l => l.amount).ToList();
        
            //Spline Chart - Income vs Expense
            //Income
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date.Date)
                .Select(k => new SplineChartData
                {
                    day = k.Key.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                })
                .ToList();

            // Expense (group by date-only)
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date.Date)
                .Select(k => new SplineChartData
                {
                    day = k.Key.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                })
                .ToList();

            // Combine Income and Expense for each day in the range
            string[] lastMonth = Enumerable.Range(0, 30)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            var merged = (from day in lastMonth
                          join income in IncomeSummary on day equals income.day into dayIncomeJoined
                          from income in dayIncomeJoined.DefaultIfEmpty()
                          join expense in ExpenseSummary on day equals expense.day into expenseJoined
                          from expense in expenseJoined.DefaultIfEmpty()
                          select new SplineChartData
                          {
                              day = day,
                              income = income == null ? 0 : income.income,
                              expense = expense == null ? 0 : expense.expense
                          }).ToList();

            ViewBag.SplineChartData = merged;

            //Recent Transactions
            ViewBag.RecentTransactions = await _context.Transactions
            .Include(i => i.Category)
            .OrderByDescending(j => j.Date)
            .Take(5)
            .ToListAsync();
            return View();
        }
    }

    public class SplineChartData
    {
        public string day { get; set; }
        public int income { get; set; }
        public int expense { get; set; }
    }
}