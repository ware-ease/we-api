using DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;


namespace DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IConfiguration _configuration;
        private WaseEaseDbContext _context;

        private AccountRepository _accountRepo;
        private GroupRepository _groupRepo;
        private ProfileRepository _profileRepo;
        private AppActionRepository _appActionRepo;


        public UnitOfWork(WaseEaseDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
        AccountRepository IUnitOfWork.AccountRepository
        {
            get
            {
                if (_accountRepo == null)
                {
                    this._accountRepo = new AccountRepository(_context, _configuration);
                }
                return _accountRepo;
            }
        } 
        GroupRepository IUnitOfWork.GroupRepository
        {
            get
            {
                if (_groupRepo == null)
                {
                    this._groupRepo = new GroupRepository(_context);
                }
                return _groupRepo;
            }
        }
        ProfileRepository IUnitOfWork.ProfileRepository
        {
            get
            {
                if (_profileRepo == null)
                {
                    this._profileRepo = new ProfileRepository(_context);
                }
                return _profileRepo;
            }
        }
        AppActionRepository IUnitOfWork.AppActionRepository
        {
            get
            {
                if (_appActionRepo == null)
                {
                    this._appActionRepo = new AppActionRepository(_context);
                }
                return _appActionRepo;
            }
        }
    }
}
