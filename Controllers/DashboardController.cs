
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
                CategoryTitleWithIcon = k.First().Category.Icon + " "+k.First().Category.Title,
                amount = k.Sum(j => j.Amount),
                formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
            });
            return View();
        }
    }
}