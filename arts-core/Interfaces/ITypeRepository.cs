using arts_core.Data;
using arts_core.Models;
using Microsoft.EntityFrameworkCore;

namespace arts_core.Interfaces
{
    public interface ITypeRepository : IRepository<Models.Type>
    {
        Task<CustomResult> GetAllType();

        CustomResult CreateNewType(Models.Type type);


    }
    public class TypeRepository : GenericRepository<Models.Type>, ITypeRepository
    {
        private readonly ILogger<TypeRepository> _logger;
        public TypeRepository(DataContext dataContext, ILogger<TypeRepository> logger) : base(dataContext)
        {
            _logger = logger;
        }

        public CustomResult CreateNewType(Models.Type type)
        {
            try
            {
                _context.Types.Add(type);
                return new CustomResult(200, "Success", type);
            }catch (Exception ex) {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> GetAllType()
        {
            try
            {
                var list = await _context.Types.ToListAsync();

                return new CustomResult(200, "Success", list);
            }catch(Exception ex) {

                return new CustomResult(200, "success", ex.Message);
            
            }

        }
    }
}
