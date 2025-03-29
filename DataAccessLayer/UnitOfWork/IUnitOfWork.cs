using Data.Entity.Base;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        void Save();
        Task<int> SaveAsync();

        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;

        IAccountRepository AccountRepository { get; }
        IGroupRepository GroupRepository { get; }
        IProfileRepository ProfileRepository { get; }
        IRouteRepository AppActionRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        //ICustomerRepository CustomerRepository { get; }
        ILocationRepository LocationRepository { get; }
        IGoodRequestRepository GoodRequestRepository { get; }
        IGoodRequestDetailRepository GoodRequestDetailRepository { get; }
        IProductRepository ProductRepository { get; }
        IPartnerRepository PartnerRepository { get; }
        IGoodNoteRepository GoodNoteRepository { get; }
        IGoodNoteDetailRepository GoodNoteDetailRepository { get; }
        IBatchRepository BatchRepository { get; }
        IInventoryRepository InventoryRepository { get; }
    }
}
