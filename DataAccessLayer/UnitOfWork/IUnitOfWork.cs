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

    }
}
