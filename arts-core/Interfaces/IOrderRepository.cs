using arts_core.Data;
using arts_core.Models;
using arts_core.ReturnModels;
using arts_core.Service;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace arts_core.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<CustomPaging> GetAllOrderAdmin(int pageNumber, int pageSize, string active, string orderId, string customer, List<string> category,
           string productCode,
          List<string> payment,
         string paymentCode,
          List<string> delivery,
          string from,
         string to, string fromDate, string toDate);
        Task<CustomResult> AcceptOrder(ICollection<int> orderId);
        Task<CustomResult> DenyOrder(ICollection<int> orderId);
        Task<CustomResult> DeliveryOrder(ICollection<int> orderId);
        Task<CustomResult> OrderSuccess(ICollection<int> orderId);
        Task<CustomResult> OrderDetail(int orderId);
        Task<CustomPaging> GetCustomerOrders(int userId, int pageNumber, int pageSize, string active, string search);
        Task<CustomResult> GetOrderDetail(int userId, int orderId);
        Task<CustomResult> CancelOrder(int userId, int orderId, string reason);
        Task<CustomPaging> GetOrderRefund(int pageNumber, int pageSize, string active);
        Task<CustomPaging> GetOrderExchage(int pageNumber, int pageSize, string active);
        public Task<CustomResult> GetMonthlyRevenue(int year);
        public Task<CustomResult> GetOrderRevenue(string option);
        public Task<CustomResult> GetNumberOfOrder(string option);
        public Task<CustomResult> GetRecentOrder();
        public CustomResult GetOrderDashBoard();
        public Task<CustomPaging> GetUserRefundExchange(int userId, int pageNumber, int pageSize, string active);
        Task<List<Order>> GetOrdersByDateAsync(DateTime dateFrom, DateTime dateTo);
    }
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ILogger<OrderRepository> _logger;
        private readonly IMailService _mailService;
        public OrderRepository(ILogger<OrderRepository> logger, DataContext dataContext, IMailService mailService) : base(dataContext)
        {
            _logger = logger;
            _mailService = mailService;
        }

        public async Task<CustomResult> AcceptOrder(ICollection<int> orderId)
        {
            try
            {
                foreach (var item in orderId)
                {
                    var order = await _context.Orders.Include(o => o.User).SingleOrDefaultAsync(o => o.Id == item);

                    if (order.OrderStatusId != 13)
                    {
                        return new CustomResult(400, "current status is not pending", null);
                    }
                    
               

                    order.OrderStatusId = 14;
                    order.UpdatedAt = DateTime.Now;
                    _context.Orders.Update(order);

                    string subject = "Arts Notification";
                    string body = $"<h1>Dear {order.User.Fullname}</h1>" +
                        $"<p>Your Order with OrderId {order.OrderCode} has been accepted</p>";
                    var mailRequest = new MailRequestNhan(order.User.Email, subject, body);

                    _ = _mailService.SendMail(mailRequest)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Some Exception in Test");
                        }
                    });
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", orderId);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> CancelOrder(int userId, int orderId, string reason)
        {
            try
            {
                var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if(order == null)
                {
                    return new CustomResult(400, "Failed", null);
                }

                if(order.OrderStatusId != 13)
                {
                    return new CustomResult(400, "Failed", null);
                }

                var variant = await _context.Variants.SingleOrDefaultAsync(v => v.Id == order.VariantId);

                variant.AvailableQuanity += order.Quanity;

                order.IsCancel = true;
                order.UpdatedAt = DateTime.Now;
                order.CancelReason = reason;
                _context.Orders.Update(order);
                _context.Variants.Update(variant);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", order);
            }catch(Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> DeliveryOrder(ICollection<int> orderId)
        {
            try
            {
                foreach (var item in orderId)
                {
                    var order = await _context.Orders.Include(o => o.User).SingleOrDefaultAsync(o => o.Id == item);

                    if (order.OrderStatusId != 14)
                    {
                        return new CustomResult(400, "current status is not accepted", null);
                    }

                    var variant = await _context.Variants.SingleOrDefaultAsync(v => v.Id == order.VariantId);

                    variant.Quanity -= order.Quanity;

                    order.OrderStatusId = 17;

                    order.UpdatedAt = DateTime.Now;
                    _context.Orders.Update(order);
                    _context.Variants.Update(variant);

                    string subject = "Arts Notification";
                    string body = $"<h1>Dear {order.User.Fullname}</h1>" +
                        $"<p>Your Order with OrderId {order.OrderCode} has been delivery</p>";
                    var mailRequest = new MailRequestNhan(order.User.Email, subject, body);

                    _ = _mailService.SendMail(mailRequest)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Some Exception in Test");
                        }
                    });
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", orderId);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> DenyOrder(ICollection<int> orderId)
        {
            try
            {
                foreach (var item in orderId)
                {
                    var order = await _context.Orders.Include(o => o.User).SingleOrDefaultAsync(o => o.Id == item);

                    if (order.OrderStatusId != 13)
                    {
                        return new CustomResult(400, "current status is not pending", null);
                    }


                    order.OrderStatusId = 15;

                    var variant = await _context.Variants.SingleOrDefaultAsync(v => v.Id == order.VariantId);
                    variant.AvailableQuanity += order.Quanity;

                    order.UpdatedAt = DateTime.Now;
                    _context.Orders.Update(order);

                    string subject = "Arts Notification";
                    string body = $"<h1>Dear {order.User.Fullname}</h1>" +
                        $"<p>Your Order with OrderId {order.OrderCode} has been denied</p>";
                    var mailRequest = new MailRequestNhan(order.User.Email, subject, body);

                    _ = _mailService.SendMail(mailRequest)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Some Exception in Test");
                        }
                    });
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", orderId);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomPaging> GetAllOrderAdmin(int pageNumber, int pageSize, string active, string orderId, string customer, List<string> category,string productCode,List<string> payment,string paymentCode,List<string> delivery,string from,string to, string fromDate, string toDate)
        {
            try
            {
                IQueryable<Order> query;

                query = _context.Orders;

                if (active == "Pending")
                {
                    query = query.Where(q => q.OrderStatusId == 13 && q.IsCancel == false);
                }

                if (active == "Accepted")
                {
                    query = query.Where(q => q.OrderStatusId == 14);
                }

                if (active == "Denied")
                {
                    query = query.Where(q => q.OrderStatusId == 15);
                }

                if (active == "Delivery")
                {
                    query = query.Where(q => q.OrderStatusId == 17);
                }

                if (active == "Success")
                {
                    query = query.Where(q => q.OrderStatusId == 16);
                }

                if (active == "Cancel")
                {
                    query = query.Where(q => q.IsCancel == true);
                }



                query = query
                       .AsNoTracking()
                       .AsSingleQuery()
                       .Include(o => o.User)
                       .Include(o => o.OrderStatusType)
                       .Include(o => o.Variant)
                       .ThenInclude(v => v.Product)
                       .ThenInclude(p => p.Category)
                       .Include(o => o.Payment)
                       .ThenInclude(p => p.PaymentType)
                       .Include(o => o.Payment)
                       .ThenInclude(p => p.DeliveryType);
                  
                      

                query = query.OrderByDescending(o => o.UpdatedAt);

                if(orderId.Length == 8)
                {
                    int order = int.Parse(orderId.TrimStart('0'));

                    query = query.Where(q => q.Id == order);
                }

                if(customer.Length != 0)
                {
                    query = query.Where(q => q.User.Fullname.ToLower().Contains(customer.ToLower()));
                }

                if(category != null)
                {
                    query = query.Where(q => category.Contains(q.Variant.Product.Category.Name));
                }

                if(productCode.Length == 5)
                {
                    int product = int.Parse(productCode.TrimStart('0'));

                    query = query.Where(q => q.Variant.Id == product);
                }

                if(payment != null)
                {
                    query = query.Where(q => payment.Contains(q.Payment.PaymentType.Name));
                }

                if(paymentCode.Length == 6)
                {
                    int paymentCodeInt = int.Parse(paymentCode.TrimStart('0'));
                    query = query.Where(q => q.Payment.Id == paymentCodeInt);
                }

                if(delivery != null)
                {
                    query = query.Where(q => delivery.Contains(q.Payment.DeliveryType.Name));
                }

                if(from.Length != 0)
                {
                    int fromValue = int.Parse(from.TrimStart('0'));

                    query = query.Where(q => q.TotalPrice > fromValue);
                }

                if (to.Length != 0)
                {
                    int toValue = int.Parse(to.TrimStart('0'));

                    query = query.Where(q => q.TotalPrice <= toValue);
                }

                if(fromDate != "")
                {
                    var date = DateTime.Parse(fromDate).Date;

                    query = query.Where(q => q.UpdatedAt.Date >= date);
                }

                if(toDate != "")
                {
                    var date = DateTime.Parse(toDate).Date;

                    query = query.Where(q => q.UpdatedAt.Date <= date);
                }


                var total = await query.CountAsync();

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
            catch (Exception ex)
            {
                return new CustomPaging()
                {
                    Status = 400,
                    Message = "Failed",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)0 / pageSize),
                    PageSize = pageSize,
                    TotalCount = 0,
                    Data = null
                };
            }

        }

        public async Task<CustomPaging> GetCustomerOrders(int userId, int pageNumber, int pageSize, string active, string search)
        {
            try
            {
                IQueryable<Order> query;

                query = _context.Orders;

                query = query.Where(o => o.UserId == userId);

                if (active == "Pending")
                {
                    query = query.Where(q => q.OrderStatusId == 13 && q.IsCancel == false);
                }

                if (active == "Accepted")
                {
                    query = query.Where(q => q.OrderStatusId == 14);
                }

                if (active == "Denied")
                {
                    query = query.Where(q => q.OrderStatusId == 15);
                }

                if (active == "Delivery")
                {
                    query = query.Where(q => q.OrderStatusId == 17);
                }

                if (active == "Success")
                {
                    query = query.Where(q => q.OrderStatusId == 16);
                }

                if(active == "Cancel")
                {
                    query = query.Where(q => q.IsCancel == true);
                }

                query = query
                       .Include(o => o.User)
                       .Include(o => o.Refund)
                       .Include(o => o.Exchange)
                       .Include(o => o.NewOrderExchange)
                       .Include(o => o.OrderStatusType)
                       .Include(o => o.Variant)
                           .ThenInclude(v => v.Product)
                           .ThenInclude(p => p.ProductImages)
                       .Include(o => o.Variant)
                           .ThenInclude(v => v.VariantAttributes)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.PaymentType)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.DeliveryType)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.Address)
                            .AsNoTracking();
   
                long orderId;
                bool success = long.TryParse(search, out orderId);

                if (success)
                {
                    if(search.Length == 16)
                    {
                        int delivery = int.Parse(search.Substring(0, 1));
                        int catgegory = int.Parse(search.Substring(1, 2).TrimStart('0'));
                        int variant = int.Parse(search.Substring(3, 5).TrimStart('0'));
                        int order = int.Parse(search.Substring(8, 8).TrimStart('0'));

                        query = query.Where(o => o.Payment.DeliveryType.Id == delivery && o.Variant.Product.CategoryId == catgegory && o.VariantId == variant && o.Id == order);
                    }
                    else
                    {
                        query = query.Where(q => q.Id == -1);
                    }
                   
               
                }
                else
                {
                    query = query.Where(o => o.Variant.Product.Name.Contains(search));
                }

                var total = query.Count();

                query = query.OrderByDescending(o => o.UpdatedAt);

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
            catch (Exception ex)
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

        public async Task<CustomResult> GetOrderDetail(int userId, int orderId)
        {
            try
            {
                var order = await _context.Orders.Include(o => o.User)
                        .Include(o => o.Refund)
                        .Include(o => o.NewOrderExchange)
                        .Include(o => o.Exchange)
                       .Include(o => o.OrderStatusType)
                       .Include(o => o.Variant)
                           .ThenInclude(v => v.Product)
                           .ThenInclude(p => p.ProductImages)
                       .Include(o => o.Variant)
                           .ThenInclude(v => v.VariantAttributes)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.PaymentType)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.DeliveryType)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.Address)
                            .AsNoTracking().SingleOrDefaultAsync(o => o.UserId == userId && o.Id == orderId);

                if(order == null)
                {
                    return new CustomResult(400, "Failed", null);
                }

                return new CustomResult(200, "Success", order);

            }catch(Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomPaging> GetOrderExchage(int pageNumber, int pageSize, string active)
        {
            try
            {
                IQueryable<Order> query;

                query = _context.Orders.Include(o => o.User)
                        .Include(o => o.Exchange)
                        .Include(o => o.NewOrderExchange)
                       .Include(o => o.OrderStatusType)
                       .Include(o => o.Variant)
                           .ThenInclude(v => v.Product)
                           .ThenInclude(v => v.Category)
                        .Include(o => o.Variant)
                           .ThenInclude(v => v.Product)
                           .ThenInclude(p => p.ProductImages)
                       .Include(o => o.Variant)
                           .ThenInclude(v => v.VariantAttributes)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.PaymentType)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.DeliveryType)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.Address).AsNoTracking();

                query = query.Where(o => o.Exchange != null || o.NewOrderExchange != null);

                if (active == "Pending")
                {
                    query = query.Where(o => o.Exchange.Status == "Pending");
                }

                if (active == "Success")
                {
                    query = query.Where(o => o.Exchange.Status == "Success" || o.NewOrderExchange.Status == "Success");
                }

                if (active == "Denied")
                {
                    query = query.Where(o => o.Exchange.Status == "Denied");
                }

                var total = query.Count();

                query = query.OrderByDescending(o => o.UpdatedAt);
             


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
            catch (Exception ex)
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

        public async Task<CustomPaging> GetOrderRefund(int pageNumber, int pageSize, string active)
        {
            try
            {
                IQueryable<Order> query;

                query = _context.Orders.Include(o => o.User)
                        .Include(o => o.Refund)
                       .Include(o => o.OrderStatusType)
                       .Include(o => o.Variant)
                           .ThenInclude(v => v.Product)
                           .ThenInclude(v => v.Category)
                        .Include(o => o.Variant)
                           .ThenInclude(v => v.Product)
                           .ThenInclude(p => p.ProductImages)
                       .Include(o => o.Variant)
                           .ThenInclude(v => v.VariantAttributes)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.PaymentType)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.DeliveryType)
                       .Include(o => o.Payment)
                            .ThenInclude(p => p.Address).AsNoTracking();

                query = query.Where(o => o.Refund != null);

                if(active == "Pending")
                {
                    query = query.Where(o => o.Refund.Status == "Pending");
                }

                if(active == "Success")
                {
                    query = query.Where(o => o.Refund.Status == "Success");
                }

                if (active == "Denied")
                {
                    query = query.Where(o => o.Refund.Status == "Denied");
                }

                var total = query.Count();

                query = query.OrderByDescending(o => o.Refund.CreatedAt);

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

        public async Task<CustomResult> OrderDetail(int orderId)
        {
            try
            {

                var order = await _context.Orders
                        .Include(o => o.User)
                        .Include(o => o.OrderStatusType)
                        .Include(o => o.Variant)
                            .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.ProductImages)
                        .Include(o => o.Variant)
                            .ThenInclude(v => v.VariantAttributes)
                        .Include(o => o.Payment)
                             .ThenInclude(p => p.PaymentType)
                        .Include(o => o.Payment)
                             .ThenInclude(p => p.DeliveryType)
                        .Include(o => o.Payment)
                             .ThenInclude(p => p.Address)
                             .AsNoTracking()
                             .SingleOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    return new CustomResult(400, "Failed", null);
                }

                return new CustomResult(200, "Success", order);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }

        public async Task<CustomResult> OrderSuccess(ICollection<int> orderId)
        {
            try
            {
                foreach (var item in orderId)
                {
                    var order = await _context.Orders.Include(o => o.Variant.Product).Include(p => p.User).SingleOrDefaultAsync(o => o.Id == item);

                    if (order.OrderStatusId != 17)
                    {
                        return new CustomResult(400, "current status is not delivery", null);
                    }

                    order.OrderStatusId = 16;

                  
                    order.UpdatedAt = DateTime.Now;
                    _context.Orders.Update(order);

                    string subject = "Arts Notification";
                    string body = $"<h1>Dear {order.User.Fullname}</h1>" +
                        $"<p>Your Order with OrderId {order.OrderCode} has been success delivery</p>"
                        + (order.Variant.Product.WarrantyDuration != 0 && order.Variant.Product.WarrantyDuration != null ? $"<p>Your product have {order.Variant.Product.WarrantyDuration} month of warranty</p>" : "");
                    var mailRequest = new MailRequestNhan(order.User.Email, subject, body);

                    _ = _mailService.SendMail(mailRequest)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Some Exception in Test");
                        }
                    });
                }

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", orderId);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> GetMonthlyRevenue(int year)
        {
            try
            {
                var query = _context.Orders
                    .Include(p => p.Refund)
                    .Include(p => p.Exchange)
                    .Include(p => p.Variant.Product.Category)
                    .Where(o => o.UpdatedAt.Year == year)
                    .Where(o => o.OrderStatusId == 16 && (o.Refund == null || o.Refund.Status != "Success") && (o.Exchange == null || o.Exchange.Status != "Success") && o.NewOrderExchange == null);

                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();

                var groupedQuery = query.GroupBy(o => o.UpdatedAt.Month)
                                .Select(o => new
                                {
                                    InNumber = o.Key,
                                    Month = dtfi.GetMonthName(o.Key),
                                    Revenue = o.Sum(o => o.TotalPrice)
                                }).OrderBy(o => o.InNumber);

                var result = await groupedQuery.ToListAsync();

                return new CustomResult(200, "Success", result);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> GetOrderRevenue(string option)
        {
            try
            {
                var query = _context.Orders
                    .Include(p => p.Refund)
                    .Include(p => p.Exchange)
                    .Include(p => p.Variant.Product.Category)
                    .Where(o => o.OrderStatusId == 16 && (o.Refund == null || o.Refund.Status != "Success" ) && (o.Exchange == null || o.Exchange.Status != "Success")  && o.NewOrderExchange == null);


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

                var groupedQuery = query.GroupBy(o => o.UpdatedAt.Date)
                                    .Select(o => new
                                    {
                                        Date = o.Key,
                                        Value = o.Sum(o => o.TotalPrice)
                                    });
                groupedQuery = groupedQuery.OrderBy(g => g.Date);

                var result = await groupedQuery.ToListAsync();

                return new CustomResult(200, "Success", result);
            }
            catch(Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            } 
        }

        public async Task<CustomResult> GetNumberOfOrder(string option)
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

                var groupedQuery = query.GroupBy(o => o.UpdatedAt.Date)
                                    .Select(o => new
                                    {
                                        Date = o.Key,
                                        Value = o.Count()
                                    });
                groupedQuery = groupedQuery.OrderBy(g => g.Date);

                var result = await groupedQuery.ToListAsync();

                return new CustomResult(200, "Success", result);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }

        public CustomResult GetOrderDashBoard()
        {
            try
            {
                var query = _context.Orders;

                var dashBoardInfo = new OrderDashBoard()
                {
                    SuccessOrder = query.Where(o => o.OrderStatusId == 16 && (o.Refund == null || o.Refund.Status != "Success") && (o.Exchange == null || o.Exchange.Status != "Success") && o.NewOrderExchange == null).Count(),
                    CancelOrder = query.Where(o => o.IsCancel == true).Count(),
                    DeliveryOrder = query.Where(o => o.OrderStatusId == 17).Count(),
                    AcceptedOrder = query.Where(o => o.OrderStatusId == 14 && o.IsCancel == false).Count(),
                    DeniedOrder = query.Where(o => o.OrderStatusId == 15).Count(),
                    ExchangeOrder = query.Where(o => o.Exchange != null && o.Exchange.Status == "Success").Count(),
                    RefundOrder = query.Where(o => o.Refund != null && o.Refund.Status == "Success").Count(),
                };

                return new CustomResult(200, "Success", dashBoardInfo);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> GetRecentOrder()
        {
            try
            {
                var query = _context.Orders
                        .AsNoTracking()
                        .AsSingleQuery()
                       .Include(o => o.User)
                       .Include(o => o.Variant)
                       .ThenInclude(v => v.Product)
                       .ThenInclude(p => p.ProductImages)
                       .Include(o => o.Variant)
                       .ThenInclude(v => v.VariantAttributes);

                var orderedQuery = query.OrderByDescending(o => o.UpdatedAt).Take(5);

                var result = await orderedQuery.ToListAsync();

                return new CustomResult(200, "Success", result);
                            
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }

        public async Task<CustomPaging> GetUserRefundExchange(int userId, int pageNumber, int pageSize, string active)
        {
            try
            {
                IQueryable<Order> query;

                query = _context.Orders
                        .AsNoTracking().
                        Include(o => o.User)
                       .Include(o => o.Variant.Product.Category)
                       .Include(v => v.Variant.Product.ProductImages)
                       .Include(o => o.Variant.VariantAttributes)
                       .Include(o => o.Payment.PaymentType)
                       .Include(o => o.Payment.DeliveryType)
                       .Include(o => o.Payment.Address)
                       .Include(o => o.Refund)
                       .Include(o => o.Exchange)
                       .Include(o => o.NewOrderExchange);

                query = query.Where(o => o.Refund != null || o.Exchange != null || o.NewOrderExchange != null);

                if (active == "Pending")
                {
                    query = query.Where(o => o.Refund.Status == "Pending" || o.Exchange.Status == "Pending" );
                }

                if (active == "Success")
                {
                    query = query.Where(o => o.Refund.Status == "Success" || o.Exchange.Status == "Success" || o.NewOrderExchange.Status == "Success");
                }

                if (active == "Denied")
                {
                    query = query.Where(o => o.Refund.Status == "Denied" || o.Exchange.Status == "Denied");
                }

                var total = await query.CountAsync();


                query = query.OrderByDescending(o => o.UpdatedAt);

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

        public async Task<List<Order>> GetOrdersByDateAsync(DateTime dateFrom, DateTime dateTo)
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderStatusType)
                .Include(o => o.Payment)
                .Include(o => o.Variant.Product)
                .Include(o => o.Exchange)
                .Include(p => p.Refund)
                .Where(o =>
                o.OrderStatusId == 16 &&
                o.CreatedAt.Date >= dateFrom.Date && o.CreatedAt < dateTo.Date &&
                 (o.Exchange == null || o.Exchange.Status != "Denied") &&
                o.NewOrderExchange == null &&
                (o.Refund == null || o.Refund.Status != "Success"))
                .OrderBy(o => o.Id)
                .ToListAsync();
            return orders;
        }
    }
}
