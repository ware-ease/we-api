﻿using Data.Entity.Base;
using DataAccessLayer.Generic;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        Task<int> SaveAsync();

        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;

        IAccountRepository AccountRepository { get; }
        IGroupRepository GroupRepository { get; }
        IProfileRepository ProfileRepository { get; }
        IRouteRepository AppActionRepository { get; }
        IPermissionRepository PermissionRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }

        ICustomerRepository CustomerRepository { get; }
    }
}
