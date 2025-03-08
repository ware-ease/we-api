using Data.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Generic
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity?> Get(string id);
        IQueryable<TEntity> Get();
        Task Add(TEntity entity);
        void Edit(TEntity entity);

        Task<IEnumerable<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null!,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null);
        Task<TEntity> GetByCondition(Expression<Func<TEntity, bool>> filter, string includeProperties = "");
        Task<IEnumerable<TEntity>> GetAllNoPaging(
            Expression<Func<TEntity, bool>> filter = null!,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null!,
            string includeProperties = "",
            string thenIncludeProperties = "");
        Task Insert(TEntity entity);
        void Delete(string id);
        void Delete(TEntity entityToDelete);
        void DeletePermanently(string id);
        void DeletePermanently(TEntity entityToDelete);
        void Update(TEntity entityToUpdate);
        Task<int> Count(Expression<Func<TEntity, bool>> filter = null!);
    }
}
