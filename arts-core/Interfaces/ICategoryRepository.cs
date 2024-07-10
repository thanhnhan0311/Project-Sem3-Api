using arts_core.Data;
using arts_core.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Runtime.CompilerServices;


namespace arts_core.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<CustomResult> GetAllCategories();

        CustomResult CreateNewCategory(Category category);

        Task<CustomResult> GetSaleByCategory(string option);
    }

    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(DataContext dataContext, ILogger<CategoryRepository> logger) : base(dataContext)
        {
            _logger = logger;
        }

        public CustomResult CreateNewCategory(Category category)
        {
            try
            {
                _context.Categories.Add(category);

                return new CustomResult(200, "success", category);

            }catch(Exception ex)
            {
                return new CustomResult(400, "failed", ex.Message);
            }
        }

        public async Task<CustomResult> GetAllCategories()
        {
            try
            {
                var list = await _context.Categories.ToListAsync();

                return new CustomResult(200, "success", list);
                
            }catch(Exception ex)
            {
                return new CustomResult(400, "bad request", ex.Message);
            }
        }

        public async Task<CustomResult> GetSaleByCategory(string option)
        {
            try
            {
                var query = _context.Orders
                    .Include(p => p.Refund)
                    .Include(p => p.Exchange)
                    .Include(p => p.Variant.Product.Category)
                    .Where(o => o.OrderStatusId == 16 && (o.Refund == null || o.Refund.Status != "Success") && (o.Exchange == null || o.Exchange.Status != "Success") && o.NewOrderExchange == null);

                if (option == "today")
                {
                    var startOfToday = DateTime.Today;
                    var startOfTomorrow = DateTime.Today.AddDays(1);
                    query = query.Where(o => o.UpdatedAt >= startOfToday && o.UpdatedAt < startOfTomorrow);
                }

                if (option == "thisweek")
                {
                    var today = DateTime.Now;
                    var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
                    var endOfThisWeek = startOfThisWeek.AddDays(7);
                    query = query.Where(o => o.UpdatedAt >= startOfThisWeek && o.UpdatedAt < endOfThisWeek);
                }

                if (option == "lastweek")
                {
                    var today = DateTime.Now;
                    var startOfLastWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday - 7);
                    var endOfLastWeek = startOfLastWeek.AddDays(7);
                    query = query.Where(o => o.UpdatedAt >= startOfLastWeek && o.UpdatedAt < endOfLastWeek);
                }


                var groupedQuery = query.GroupBy(o => o.Variant.Product.Category.Name)
                                        .Select(o => new
                                        {
                                            Id = o.Key,
                                            Label = o.Key,
                                            Value = o.Sum(o => o.TotalPrice)
                                        });
                          
                var result = await groupedQuery.ToListAsync();

                return new CustomResult(200, "Success", result);



            } catch(Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        private static int GetWeekOfYear(DateTime date)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            Calendar calendar = cultureInfo.Calendar;
            CalendarWeekRule weekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            return calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
        }
    }

  
}
