﻿using Data.Entity;
using DataAccessLayer.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetFullProductById(string id);

        /*IQueryable<Product> GetAllQueryable();
        Task<Product> GetByIdAsync(string id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        IQueryable<Product> GetByCategoryIdQueryable(string categoryId);*/
    }
}
