using Data.Entity.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Generic
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        internal WaseEaseDbContext _context;
        internal DbSet<TEntity> _dbSet;

        public GenericRepository(WaseEaseDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> Get(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<TEntity> Get()
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            var entityType = _context.Model.FindEntityType(typeof(TEntity));

            if (entityType != null)
            {
                foreach (var navigation in entityType.GetNavigations())
                {
                    query = query.Include(navigation.Name);

                    foreach (var subNavigation in navigation.TargetEntityType.GetNavigations())
                    {
                        var secondLevelInclude = $"{navigation.Name}.{subNavigation.Name}";

                        query = query.Include(secondLevelInclude);

                        foreach (var subSubNavigation in subNavigation.TargetEntityType.GetNavigations())
                        {
                            var thirdLevelInclude = $"{secondLevelInclude}.{subSubNavigation.Name}";
                            query = query.Include(thirdLevelInclude);
                        }
                    }
                }
            }

            return query;
        }

        public async Task Add(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Edit(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null!,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
            string includeProperties = "",
            int? pageIndex = null, // Optional parameter for pagination (page number)
            int? pageSize = null)  // Optional parameter for pagination (number of records per page)
        {
            IQueryable<TEntity> query = _dbSet;

            // Nếu entity có IsDeleted, tự động lọc những bản ghi chưa bị xóa
            if (HasIsDeletedProperty())
            {
                var isDeletedFilter = (Expression<Func<TEntity, bool>>)DynamicExpressionParser.ParseLambda(
                    typeof(TEntity), typeof(bool), "IsDeleted == false");

                query = query.Where((Expression<Func<TEntity, bool>>)isDeletedFilter);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!includeProperties.IsNullOrEmpty())
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                // Ensure the pageIndex and pageSize are valid
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public virtual async Task<TEntity> GetByID(string id)
        {
            return await _dbSet.FindAsync(id)!;
        }

        public virtual async Task<TEntity> GetByCondition(Expression<Func<TEntity, bool>> filter, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            // Nếu entity có IsDeleted, tự động lọc những bản ghi chưa bị xóa
            if (HasIsDeletedProperty())
            {
                var isDeletedFilter = (Expression<Func<TEntity, bool>>)DynamicExpressionParser.ParseLambda(
                    typeof(TEntity), typeof(bool), "IsDeleted == false");

                query = query.Where((Expression<Func<TEntity, bool>>)isDeletedFilter);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!includeProperties.IsNullOrEmpty())
            {

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync()!;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllNoPaging(
            Expression<Func<TEntity, bool>> filter = null!,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
            string includeProperties = "",
            string thenIncludeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;


            // Nếu entity có IsDeleted, tự động lọc những bản ghi chưa bị xóa
            if (HasIsDeletedProperty())
            {
                var isDeletedFilter = (Expression<Func<TEntity, bool>>)DynamicExpressionParser.ParseLambda(
                    typeof(TEntity), typeof(bool), "IsDeleted == false");

                query = query.Where((Expression<Func<TEntity, bool>>)isDeletedFilter);
            }


            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (!includeProperties.IsNullOrEmpty())
            {

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            if (!thenIncludeProperties.IsNullOrEmpty())
            {

                foreach (var thenIncludeProperty in thenIncludeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(thenIncludeProperty);
                }
            }
            return await query.AsNoTracking().ToListAsync();

        }


        public virtual async Task Insert(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id)!;
            Delete(entityToDelete!);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> filter = null!)
        {
            IQueryable<TEntity> query = _dbSet;

            // Nếu entity có IsDeleted, tự động lọc những bản ghi chưa bị xóa
            if (HasIsDeletedProperty())
            {
                var isDeletedFilter = (Expression<Func<TEntity, bool>>)DynamicExpressionParser.ParseLambda(
                    typeof(TEntity), typeof(bool), "IsDeleted == false");

                query = query.Where((Expression<Func<TEntity, bool>>)isDeletedFilter);
            }


            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.CountAsync();
        }

        private static bool HasIsDeletedProperty()
        {
            return typeof(TEntity).GetProperty("IsDeleted") != null;
        }
    }
}
