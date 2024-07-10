using arts_core.Data;

namespace arts_core.Interfaces
{
    //nhom tat ca repositories lại với nhau để đám bảo tính nhất quán của dữ liệu
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ITypeRepository TypeRepository { get; }

        IProductRepository ProductRepository { get; }
        ICartRepository CartRepository { get; }

        IAddressRepository AddressRepository { get; }

        IReviewRepository ReviewRepository { get; }

        IOrderRepository OrderRepository { get; }

        IPaymentRepository PaymentRepository { get; }

        IRefundRepository RefundRepository { get; }

        IExchangeRepository ExchangeRepository { get; }

        void SaveChanges();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        public IUserRepository UserRepository { get; set; }
        public ICategoryRepository CategoryRepository { get; set; }
        public ITypeRepository TypeRepository { get; set; }
        public IProductRepository ProductRepository { get; set; }
        public ICartRepository CartRepository { get; set; }
        public IAddressRepository AddressRepository { get; set; }
        public IReviewRepository ReviewRepository { get; set; }

        public IPaymentRepository PaymentRepository { get; set; }
        public IOrderRepository OrderRepository { get; set; }

        public IRefundRepository RefundRepository { get; set; }

        public IExchangeRepository ExchangeRepository { get; set; }

        public UnitOfWork(DataContext dataContext, IUserRepository userRepository, ICategoryRepository categoryRepository, ITypeRepository typeRepository, IProductRepository productRepository, ICartRepository cartRepository, IAddressRepository addressRepository, IReviewRepository reviewRepository, IOrderRepository orderRepository, IPaymentRepository paymentRepository, IRefundRepository refundRepository, IExchangeRepository exchangeRepository)
        {
            _dataContext = dataContext;
            UserRepository = userRepository;
            CategoryRepository = categoryRepository;
            TypeRepository = typeRepository;
            ProductRepository = productRepository;
            CartRepository = cartRepository;
            AddressRepository = addressRepository;
            ReviewRepository = reviewRepository;
            OrderRepository = orderRepository;
            PaymentRepository = paymentRepository;
            RefundRepository = refundRepository;
            ExchangeRepository = exchangeRepository;
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }

        public void SaveChanges()
        {
            _dataContext.SaveChanges();
        }
    }
}
