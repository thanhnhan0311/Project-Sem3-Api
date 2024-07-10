using arts_core.Data;
using arts_core.Models;
using Microsoft.EntityFrameworkCore;

namespace arts_core.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<CustomPaging> GetUserPayments(int userId, int pageNumber, int pageSize);  
      

    }
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;
        public PaymentRepository(ILogger<PaymentRepository> logger, DataContext dataContext) : base(dataContext)
        {
            _logger = logger;
        }

        public async Task<CustomPaging> GetUserPayments(int userId, int pageNumber, int pageSize)
        {
            try
            {
                IQueryable<Payment> query;

                query = _context.Payments.Include(p => p.Address).ThenInclude(a => a.User);

                query = query.Where(p => p.Address.UserId == userId);

                query = query.Include(p => p.Orders)
                                .ThenInclude(p => p.Variant)
                                    .ThenInclude(v => v.Product)
                                        .ThenInclude(p => p.ProductImages)
                                   .Include(p => p.Orders)
                                .ThenInclude(p => p.Variant)
                                .ThenInclude(v => v.VariantAttributes)
                                .Include(p => p.Orders)
                                    .ThenInclude(o => o.OrderStatusType)
                                .Include(p => p.PaymentType)
                                .Include(p => p.DeliveryType)
                                .Include(p => p.Orders).ThenInclude(o=>o.Exchange)
                                .Include(p => p.Orders).ThenInclude(o => o.NewOrderExchange)
                                .Include(p => p.Orders).ThenInclude(o => o.Refund)
                                .AsNoTracking();

                var total = query.Count();

                query = query.OrderByDescending(p => p.CreatedAt);

                query = query.Skip((pageNumber - 1) * pageSize)
                      .Take(pageSize);

                var list = await query.ToListAsync();

                var customPaging = new CustomPaging()
                {
                    Status = 200,
                    Message = "OK",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    PageSize = pageSize,
                    TotalCount = total,
                    Data = list
                };

                return customPaging;

            }
            catch(Exception ex)
            {
                return new CustomPaging()
                {
                    Status = 400,
                    Message = "Failed",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)0 / pageSize),
                    PageSize = pageSize,
                    TotalCount = 0,
                    Data = ex.Message
                };
            }
        }

    }
}
