using arts_core.Data;
using arts_core.Models;
using arts_core.Service;
using Microsoft.EntityFrameworkCore;

namespace arts_core.Interfaces
{
    public interface IRefundRepository : IRepository<Refund>
    {
        Task<CustomResult> CreateRefundAsync(RefundRequest request);
        Task<CustomResult> GetRefundsByUserIdAsync(int userId);
        Task<CustomResult> GetAllRefundsAsync();
        Task<CustomResult> UpdateRefundAsync(RefundReQuestForAdmin request);
        Task<CustomResult> GetRefundById(int refundId);
        Task<CustomResult> GetUserRefundById(int userId, int refundId);

    }
    public class RefundRepository : GenericRepository<Refund>, IRefundRepository
    {
        private readonly ILogger<CartRepository> _logger;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        public RefundRepository(ILogger<CartRepository> logger, DataContext dataContext, IFileService fileService, IMailService mailService) : base(dataContext)
        {
            _logger = logger;
            _fileService = fileService;
            _mailService = mailService;
        }
        public async Task<CustomResult> CreateRefundAsync(RefundRequest request)
        {
            bool isExpired = false;
            try
            {
                //check refund expired 
                var order = await _context.Orders.Include(od => od.Variant).FirstOrDefaultAsync(o => o.Id == request.OrderId);
                isExpired = isOrderOlderThan7Days(order);
                if (isExpired)
                    return new CustomResult(401, "Order must be within 7 days to Refund", null);

                //kiem tra co order nao da tung refund khong
                var refunds = await _context.Refunds.Where(r => r.OrderId == request.OrderId).FirstOrDefaultAsync();
                if (refunds != null)
                    return new CustomResult(402, "Order had been refund before", null);

                var images = new List<StoreImage>();
                if (request.Images != null)
                {
                    var imageRoots = await _fileService.StoreImageAsync("Images", request.Images);
                    foreach (var imageRoot in imageRoots)
                    {
                        var imageName = imageRoot;
                        string entityName = "Refunds";
                        var storeImage = new StoreImage()
                        {
                            EntityName = entityName,
                            ImageName = imageName
                        };
                        images.Add(storeImage);
                    }
                    _logger.LogInformation($"filename: {imageRoots}");
                }

                var refund = new Refund() { 
                    OrderId = order.Id, 
                    ReasonRefund = request.ReasonRefund, 
                    AmountRefund = (float)order.TotalPrice,
                    Images = images
                };


                _context.Refunds.Add(refund);
                order.UpdatedAt = DateTime.Now;
                _context.Orders.Update(order);  
                await _context.SaveChangesAsync();
                return new CustomResult(200, "Your refund has been delivery success", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrong in CreateRefund");
                throw;
            }
        }

        public async Task<CustomResult> GetRefundsByUserIdAsync(int userId)
        {
            try
            {
                var refunds = await _context.Refunds.Include(r => r.Order).OrderByDescending(r => r.CreatedAt).Where(r => r.Order.UserId == userId).ToListAsync();

                if (refunds.Count == 0)
                    return new CustomResult(400, "Not Found", refunds);

                return new CustomResult(200, "Get successfull", refunds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrong in GetRefundsByUserId");
                throw;
            }
        }
        public async Task<CustomResult> GetAllRefundsAsync()
        {
            try
            {
                var refunds = await _context.Refunds.Include(r => r.Order).OrderByDescending(r => r.CreatedAt).ToListAsync();
                return new CustomResult(200, "Get All Refunds", refunds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrong in GetAllRefundsAsync");
                throw;
            }
        }
        public async Task<CustomResult> UpdateRefundAsync(RefundReQuestForAdmin request)
        {
            try
            {
                var refund = await _context.Refunds
                    .Include(r => r.Order)
                    .Include(r => r.Order.Payment)
                    .Include(r => r.Order.User)
                    .Include(r => r.Order.Variant.Product)
                    .FirstOrDefaultAsync(r => r.Id == request.RefundId);
                if (refund == null)
                    return new CustomResult(400, "Refund Not Found", null);

                if (refund.Status == "Success" || refund.Status == "Denied")
                    return new CustomResult(401, "Refund Can't Update when success or denied", null);

                refund.ResponseRefund = request.ResponseRefund;
                refund.Status = request.Status;
                refund.UpdatedAt = DateTime.Now;
                _context.Refunds.Update(refund);
                await _context.SaveChangesAsync();

                /// send Mail
                var name = refund.Order.User.Fullname;
                var email = refund.Order.User.Email;
                var orderCode = refund.Order.OrderCode;
                string subject = "Arts Notification";
                string body = $"<h1>Dear {name}</h1>" +
                    $"<p>Your refund with OrderId {orderCode} has been  {request.Status}</p>" +
                    $"<p>Reason: {request.ResponseRefund}</p>";
                var mailRequest = new MailRequestNhan(email, subject, body);

                _ = _mailService.SendMail(mailRequest)
              .ContinueWith(t =>
              {
                  if (t.IsFaulted)
                  {
                      _logger.LogError(t.Exception, "Some Exception in Test");
                  }
              });

                return new CustomResult(200, "Update refund is Sucess", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something went wrong in UpdateRefundAsync");
                throw;
            }

        }

    

        private bool isOrderOlderThan7Days(Order order)
        {
            return (DateTime.Now - order.CreatedAt).TotalDays > 7;
        }

        public async Task<CustomResult> GetRefundById(int refundId)
        {
            try
            {
                var refund = await _context.Refunds.Include(r => r.Images).Include(r => r.Order).SingleOrDefaultAsync(r => r.Id == refundId);


                return new CustomResult(200, "Success", refund);

            }catch(Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> GetUserRefundById(int userId, int refundId)
        {
            try
            {
                var refund = await _context.Refunds.Include(r => r.Images).Include(r => r.Order).SingleOrDefaultAsync(r => r.Id == refundId && r.Order.UserId == userId);

                return new CustomResult(200, "Success", refund);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }
    }
    public class RefundRequest
    {
        public int OrderId { get; set; }
        public string ReasonRefund { get; set; } = string.Empty;
        public ICollection<IFormFile>? Images { get; set; }

    }
    public class RefundReQuestForAdmin
    {
        public int RefundId { get; set; }
        public string? ResponseRefund { get; set; }
        public string Status { get; set; }
   
    }
}