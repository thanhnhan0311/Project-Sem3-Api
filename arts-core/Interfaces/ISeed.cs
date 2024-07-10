using arts_core.Data;
using arts_core.Models;
using arts_core.Service;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace arts_core.Interfaces
{
    public interface ISeed
    {
        void SeedProductAndVariantData();
        void SeedUser();
        void SeedVariantAttribute();
        void SeedAddress();
        void SeedPayments();
        void SeedOrders();
        void SeedReview();
        void SeedProductOfCategory(string jsonUrl, int categoryId, string imageUrl);

        Task SeedOrderGiu();
        Task SeedUsersGiu();
        

    }

    public class Seed : ISeed
    {
        private DataContext _context;
        private IHostEnvironment _env;
        private ILogger<Seed> _logger;
        private IWebHostEnvironment _webHostEnvironment;
        private IFileService _fileService;
        public Seed(DataContext dataContext,
            IHostEnvironment env,
            ILogger<Seed> logger,
            IWebHostEnvironment webHostEnvironment,
               IFileService fileService
            )
        {
            _context = dataContext;
            _env = env;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        public void SeedProductAndVariantData()
        {
            try
            {
                var rootPath = _env.ContentRootPath;
                var fullPath = Path.Combine(rootPath, "Data/product.json");

                var jsonData = System.IO.File.ReadAllText(fullPath);
                _logger.LogInformation(jsonData);
                if (string.IsNullOrWhiteSpace(jsonData))
                    _logger.LogError("Is Null Or WhiteSpace in Seed Product");

                var products = JsonConvert.DeserializeObject<List<Product>>(jsonData);
                if (products == null || products.Count == 0)
                    _logger.LogError("Product is Null To Seed");


                _context.Products.AddRange(products);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in Seed");
            }
        }

        public void SeedUser()
        {
            try
            {
                var rootPath = _env.ContentRootPath;
                var fullPath = Path.Combine(rootPath, "Data/user.json");
                var jsonData = System.IO.File.ReadAllText(fullPath);
                _logger.LogInformation(jsonData);
                if (string.IsNullOrWhiteSpace(jsonData))
                    _logger.LogError("Is Null Or WhiteSpace in Seed User");

                var users = JsonConvert.DeserializeObject<List<User>>(jsonData);
                if (users == null || users.Count == 0)
                    _logger.LogError("user is Null to Seed");
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in Seed");
            }
        }

        public void SeedVariantAttribute()
        {
            try
            {
                var rootPath = _env.ContentRootPath;
                var fullPath = Path.Combine(rootPath, "Data/variantAttributes.json");
                var jsonData = System.IO.File.ReadAllText(fullPath);
                _logger.LogInformation(jsonData);
                if (string.IsNullOrWhiteSpace(jsonData))
                    _logger.LogError("Is Null Or WhiteSpace in Seed User");

                var variantAttribues = JsonConvert.DeserializeObject<List<VariantAttribute>>(jsonData);
                if (variantAttribues == null || variantAttribues.Count == 0)
                    _logger.LogError("user is Null to Seed");
                _context.VariantAttributes.AddRange(variantAttribues);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in Seed");
            }
        }
        public void SeedAddress()
        {
            try
            {
                var rootPath = _env.ContentRootPath;
                var fullPath = Path.Combine(rootPath, "Data/address.json");
                var jsonData = System.IO.File.ReadAllText(fullPath);
                _logger.LogInformation(jsonData);
                if (string.IsNullOrWhiteSpace(jsonData))
                    _logger.LogError("Is Null Or WhiteSpace in Seed Address");
                var addressess = JsonConvert.DeserializeObject<List<Address>>(jsonData);
                if (addressess == null || addressess.Count == 0)
                    _logger.LogError("address is Null to Seed");
                _context.Addresses.AddRange(addressess);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in Seed");
            }
        }
        public void SeedPayments()
        {
            try
            {
                var rootPath = _env.ContentRootPath;
                var fullPath = Path.Combine(rootPath, "Data/payments.json");
                var jsonData = System.IO.File.ReadAllText(fullPath);
                _logger.LogInformation(jsonData);
                if (string.IsNullOrWhiteSpace(jsonData))
                    _logger.LogError("Is Null Or WhiteSpace in Seed User");

                var payments = JsonConvert.DeserializeObject<List<Payment>>(jsonData);
                if (payments == null || payments.Count == 0)
                    _logger.LogError("payments is Null to Seed");
                _context.Payments.AddRange(payments);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in Seed");
            }
        }
        public void SeedOrders()
        {
            try
            {
                var rootPath = _env.ContentRootPath;
                var fullPath = Path.Combine(rootPath, "Data/orders.json");
                var jsonData = System.IO.File.ReadAllText(fullPath);
                _logger.LogInformation(jsonData);
                if (string.IsNullOrWhiteSpace(jsonData))
                    _logger.LogError("Is Null Or WhiteSpace in Seed User");

                var orders = JsonConvert.DeserializeObject<List<Order>>(jsonData);
                if (orders == null || orders.Count == 0)
                    _logger.LogError("orders is Null to Seed");
                _context.Orders.AddRange(orders);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in Seed");
            }
        }
        public void SeedReview()
        {
            var orders = _context.Orders
                .Include(o => o.Variant).ThenInclude(v => v.Product)
                .Include(o => o.User).ToList();
            foreach (var order in orders)
            {
                order.Review = new Review()
                {
                    UserId = order.UserId,
                    ProductId = order.Variant.ProductId,
                    Rating = 4,
                    Comment = "Oke"
                };
            }
            _context.Orders.UpdateRange(orders);
            var result = _context.SaveChanges();


        }
        public void SeedProductOfCategory(string jsonUrl, int categoryId, string imageUrl)
        {
            try
            {
                var rootPath = _env.ContentRootPath;
                var imageRootPath = _webHostEnvironment.WebRootPath;

                var fullPath = Path.Combine(rootPath, $"Data/{jsonUrl}");
                var imagePath = Path.Combine(imageRootPath, $"Image/{imageUrl}");


                var imagesName = _fileService.GetFilesName(imagePath);


                var jsonData = System.IO.File.ReadAllText(fullPath);

                if (string.IsNullOrWhiteSpace(jsonData))
                    _logger.LogError("Is Null Or WhiteSpace in Seed Product");


                var productsCrawl = JsonConvert.DeserializeObject<List<ProductCrawl>>(jsonData);

                var random = new Random();

                foreach (var item in productsCrawl)
                {
                    //string imageName = "";
                    int quanity = GetQuanity();
                    float price = random.Next(50, 200);
                    if (item.Price != null)
                    {
                        price = item.Price.Value;
                    }

                    var product = new Product()
                    {
                        Id = 0,
                        Name = item.Title,
                        CreatedAt = DateTime.Now,
                        IsActive = true,
                        CategoryId = categoryId,
                        Description = item.Description,
                        ProductImages = new List<ProductImage>()
                        {
                            new ProductImage()
                            {
                                Id = 0,
                                ImageName =  GetImageName(imagesName)
                            }
                        },
                        Variants = new List<Variant>()
                        {
                            new Variant()
                            {
                                Id = 0,
                                VariantImage = "",
                                Quanity = quanity,
                                AvailableQuanity = GetAvailableQuanity(quanity),
                                Price = price,
                                SalePrice = GetSalePrice(price),
                                CreatedAt = DateTime.Now,
                                Active = true,
                            }
                        }
                    };
                    _context.Products.Add(product);
                }

                //var products = JsonConvert.DeserializeObject<List<Product>>(jsonData);
                //if (products == null || products.Count == 0)
                //    _logger.LogError("Product is Null To Seed");

                //_context.Products.AddRange(products);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in Seed");
            }
        }


        public async Task SeedOrderGiu()
        {
            try
            {
                var random = new Random();
                var users = await _context.Users.Where(u => u.RoleTypeId == 6).ToListAsync();
                foreach (var user in users)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        int randomVariant = random.Next(264, 326);

                        var variant = await _context.Variants.Where(v => v.Id == randomVariant).SingleOrDefaultAsync();
                        var quanityVariantOrder = 1;
                        var addressUser = await _context.Addresses.Where(u => u.UserId == user.Id).Take(1).SingleOrDefaultAsync();

                        var totalPrice = variant?.Price * quanityVariantOrder;

                        if (variant?.AvailableQuanity > 0)
                        {

                            var randomDate = GetRandomDate();
                            var payment = new Payment()
                            {
                                DeliveryTypeId = random.Next(1, 3),
                                AddressId = addressUser.Id,
                                PaymentTypeId = random.Next(7, 11),
                                ShipFee = 5,
                                CreatedAt = randomDate,
                            };


                            var review = new Review()
                            {
                                Comment = Faker.Lorem.Sentence(),
                                Rating = random.Next(1, 6),
                                UserId = user.Id,
                                ProductId = variant.ProductId,
                                CreatedAt = randomDate
                            };
                            var order = new Order()
                            {
                                UserId = user.Id,
                                Quanity = quanityVariantOrder,
                                OrderStatusId = 16,
                                TotalPrice = totalPrice,
                                Payment = payment,
                                VariantId = variant.Id,
                                Review = review,
                                CreatedAt = randomDate,
                                UpdatedAt = randomDate
                            };

                            variant.Quanity = variant.Quanity - quanityVariantOrder;
                            variant.AvailableQuanity = variant.AvailableQuanity - quanityVariantOrder;


                            _context.Payments.Add(payment);
                            _context.Orders.Add(order);
                            _context.Reviews.Add(review);

                            _context.Variants.Update(variant);

                            await _context.SaveChangesAsync();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something wrong in Seed");
            }
        }
  
        public async Task SeedUsersGiu()
        {
            for (int i = 0; i < 100; i++)
            {
                var user = new User()
                {
                    Email = Faker.Name.First().ToLower() + Faker.Name.Suffix().ToLower() + "@gmail.com",
                    Fullname = Faker.Name.First(),
                    Password = "$2a$12$C1JQMjVl3bfXxtISNXv9Sulqsu/nEOx.yhtIozwvUW/DHWevhhQYG",
                    PhoneNumber = Faker.Phone.Number().Replace("-", ""),
                    RoleTypeId = 6,
                    Verifired = true,

                };
                var address = new Models.Address()
                {
                    FullName = user.Fullname,
                    PhoneNumber = user.PhoneNumber,
                    AddressDetail = Faker.Address.StreetAddress(),
                    Province = "01",
                    District = "001",
                    Ward = "00001",
                    IsDefault = true,
                    User = user,
                };
                _context.Addresses.Add(address);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

        }



        private float GetSalePrice(float price)
        {
            var random = new Random();
            if (GetBoolean())
            {
                var percentSale = random.Next(1, 30);
                var priceAfterSale = price + price * percentSale / 100;
                return priceAfterSale;
            }
            return 0;
        }
        private int GetAvailableQuanity(int quantity)
        {
            var random = new Random();
            if (GetBoolean())
            {
                var quanityToDecrease = random.Next(0, quantity / 2);
                return quantity - quanityToDecrease;
            }
            return quantity;
        }

        private DateTime GetRandomDate()
        {
            DateTime startDate = new DateTime(2024, 4, 1);
            DateTime endDate = new DateTime(2024, 7, 11);

            Random random = new Random();
            int range = (endDate - startDate).Days;

            return startDate.AddDays(random.Next(range + 1));
        }
        private int GetQuanity()
        {
            var random = new Random();
            return random.Next(50, 300);
        }

        private bool GetBoolean()
        {
            var random = new Random();
            return random.Next(2) == 0;
        }
        private string GetImageName(List<string> imagesFileName)
        {
            var random = new Random();
            var randomIndex = random.Next(imagesFileName.Count);
            return imagesFileName[randomIndex];
        }
    }

    public class Price
    {
        public float Value { get; set; }
        public string Currency { get; set; } = string.Empty;
    }

    public class ProductCrawl
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Price Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }

}