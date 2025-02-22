using DataAccessLayer.Repositories;

namespace DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        Task<int> SaveAsync();
        public AccountRepository AccountRepository { get; }
        public GroupRepository GroupRepository { get; }
        public ProfileRepository ProfileRepository { get; }
        public AppActionRepository AppActionRepository { get; }
        public PermissionRepository PermissionRepository { get; }
        public WarehouseRepository WarehouseRepository { get; }
        public AccountGroupRepository AccountGroupRepository { get; }
        public AccountPermissionRepository AccountPermissionRepository { get; }
        public PermissionActionRepository PermissionActionRepository { get; }
        public GroupPermissionRepository GroupPermissionRepository { get; }
        public AccountWarehouseRepository AccountWarehouseRepository { get; }

    }
}
