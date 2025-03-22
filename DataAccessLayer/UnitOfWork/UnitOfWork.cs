using Data.Entity.Base;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private WaseEaseDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(WaseEaseDbContext context, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _context = context;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //public ICustomerRepository CustomerRepository => GetIRepository<ICustomerRepository>();
        public IRefreshTokenRepository RefreshTokenRepository => GetIRepository<IRefreshTokenRepository>();
        public IAccountRepository AccountRepository => GetIRepository<IAccountRepository>();
        public IGroupRepository GroupRepository => GetIRepository<IGroupRepository>();
        public IProfileRepository ProfileRepository => GetIRepository<IProfileRepository>();
        public IRouteRepository AppActionRepository => GetIRepository<IRouteRepository>();
        public IPermissionRepository PermissionRepository => GetIRepository<IPermissionRepository>();
        public IWarehouseRepository WarehouseRepository => GetIRepository<IWarehouseRepository>();
        public ILocationRepository LocationRepository => GetIRepository<ILocationRepository>();
        public IGoodRequestRepository GoodRequestRepository => GetIRepository<IGoodRequestRepository>();
        public IGoodRequestDetailRepository GoodRequestDetailRepository => GetIRepository<IGoodRequestDetailRepository>();
        public IProductRepository ProductRepository => GetIRepository<IProductRepository>();
        public IPartnerRepository PartnerRepository => GetIRepository<IPartnerRepository>();
        public IGoodNoteRepository GoodNoteRepository => GetIRepository<IGoodNoteRepository>();
        public IGoodNoteDetailRepository GoodNoteDetailRepository => GetIRepository<IGoodNoteDetailRepository>();
        public IBatchRepository BatchRepository => GetIRepository<IBatchRepository>();
        public IInventoryRepository InventoryRepository => GetIRepository<IInventoryRepository>();

        public TRepository GetIRepository<TRepository>()
        {
            if (_serviceProvider != null)
            {
                var repository = _serviceProvider.GetService<TRepository>();
                return repository;
            }

            return default;
        }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            var specificRepositoryType = typeof(IGenericRepository<TEntity>);
            var specificRepository = _serviceProvider.GetService(specificRepositoryType);

            if (specificRepository != null)
            {
                return (IGenericRepository<TEntity>)specificRepository;
            }

            Type entityType = typeof(TEntity);

            if (!_repositories.ContainsKey(entityType))
            {
                var repository = _serviceProvider.GetRequiredService<IGenericRepository<TEntity>>();
                _repositories[entityType] = repository;
            }

            return (IGenericRepository<TEntity>)_repositories[entityType];
        }
    }
}
