using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BomRainB.Data.Contracts
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties);

        List<TEntity> GetList(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties);

        IQueryable<TEntity> GetQuery(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeProperties);

        int Count(
            Expression<Func<TEntity, bool>> filter = null);

        TEntity GetByID(object id);

        TEntity GetByID(object[] ids);

        void Insert(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entity);

        void Update(TEntity entity);

        void InsertUnchanged(TEntity entity);

        void Detach(TEntity entity);

        void Attach(TEntity entity);

        void Save();

        bool Exists(Expression<Func<TEntity, bool>> filter = null);

        IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters);

        IEnumerable<TEntity> GetWithProcedure(string procedureName, params KeyValuePair<string, object>[] parameters);

        IEnumerable<TComplex> GetWithProcedure<TComplex>(string procedureName, params KeyValuePair<string, object>[] parameters) where TComplex : class;

        int ExecuteStoredProcedure(string procedureName, params KeyValuePair<string, object>[] parameters);
    }
}
