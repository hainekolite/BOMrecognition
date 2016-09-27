using BomRainB.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.Data.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly BomRainBirdDbContext context;
        private readonly DbSet<TEntity> dbSet;
        private bool _disposed = false;

        internal BomRainBirdDbContext Context
        {
            get { return context; }
        }

        public DbSet<TEntity> Table { get { return dbSet; } }

        public GenericRepository(BomRainBirdDbContext BomRainBirdDbContext)
        {
            context = BomRainBirdDbContext;
            dbSet = context.Set<TEntity>();
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , params string[] includeProperties)
        {
            return GetQuery(filter, orderBy, includeProperties).AsNoTracking();
        }

        public virtual List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , params string[] includeProperties)
        {
            return GetQuery(filter, orderBy, includeProperties).AsNoTracking().ToList();
        }

        public virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , params string[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter).AsNoTracking();
            }

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty).AsNoTracking();
            }
            if (orderBy != null)
            {
                return orderBy(query).AsNoTracking();
            }
            return query.AsNoTracking();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Count();
        }

        private TEntity GetByID(object id, bool detach = true)
        {
            var instance = dbSet.Find(id);
            if (detach && instance != null)
            {
                Context.Entry(instance).State = EntityState.Detached;
            }
            return instance;
        }


        private TEntity GetByID(object[] ids, bool detach = true)
        {
            var instance = dbSet.Find(ids);
            if (detach && instance != null)
            {
                Context.Entry(instance).State = EntityState.Detached;
            }
            return instance;
        }

        public virtual TEntity GetByID(object[] ids)
        {
            bool detach = true;
            return GetByID(ids, detach);
        }

        public virtual TEntity GetByID(object id)
        {
            bool detach = true;//Para evitar que se haga tracking de los objetos
            return GetByID(id, detach);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void InsertUnchanged(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Unchanged;
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            Delete(GetByID(id));
        }

        public virtual void Delete(TEntity entity)
        {
            var entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var key = GetPrimaryKeys(entity);
                var currentEntry = GetByID(key, false);
                if (currentEntry != null)
                {
                    var attachedEntry = Context.Entry(currentEntry);
                    attachedEntry.State = EntityState.Deleted;
                }
                else
                {
                    dbSet.Attach(entity);
                    Context.Entry(entity).State = EntityState.Deleted;
                }
            }
            else if (entry.State == EntityState.Deleted)
            {
                Context.Entry(entity).State = EntityState.Deleted;
            }
            System.Diagnostics.Debug.WriteLine(entry.State);
        }

        public virtual void Update(TEntity entity)
        {
            var entry = context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var key = GetPrimaryKeys(entity);
                var currentEntry = GetByID(key, false);
                if (currentEntry != null)
                {
                    var attachedEntry = Context.Entry(currentEntry);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    dbSet.Attach(entity);
                    Context.Entry(entity).State = EntityState.Modified;
                }
            }
            else if (entry.State == EntityState.Unchanged)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
        }


        public virtual IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).AsNoTracking();//context.Database.SqlQuery<TEntity>(query, parameters);
        }

        public virtual IEnumerable<TEntity> GetWithProcedure(string procedureName, params KeyValuePair<string, object>[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                return context.Database.SqlQuery<TEntity>(GetCommandText(procedureName, System.Data.CommandType.StoredProcedure, parameters), GetNamedParameters(parameters).ToArray());
            }
            return context.Database.SqlQuery<TEntity>(GetCommandText(procedureName, System.Data.CommandType.StoredProcedure, parameters));
        }

        public IEnumerable<TComplex> GetWithProcedure<TComplex>(string procedureName, params KeyValuePair<string, object>[] parameters) where TComplex : class
        {
            string cmdText = GetCommandText(procedureName, System.Data.CommandType.StoredProcedure, parameters);
            if (parameters != null && parameters.Length > 0)
            {
                return context.Database.SqlQuery<TComplex>(cmdText, GetNamedParameters(parameters).ToArray());
            }
            return context.Database.SqlQuery<TComplex>(cmdText);
        }

        public virtual int ExecuteStoredProcedure(string procedureName, params KeyValuePair<string, object>[] parameters)
        {
            string cmdText = GetCommandText(procedureName, CommandType.StoredProcedure, parameters);
            //_context.Database.CommandTimeout = StaticConfiguration.CommandTimeout;
            int identity = 0;

            if (parameters != null && parameters.Length > 0)
            {
                identity = context.Database.ExecuteSqlCommand(cmdText, GetNamedParameters(parameters).ToArray());
            }
            else
            {
                identity = context.Database.ExecuteSqlCommand(cmdText);
            }

            return identity;
        }

        public virtual void Save()
        {
            context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region - Protected Methods -
        protected List<DbParameter> GetNamedParameters(KeyValuePair<string, object>[] parameters)
        {
            List<DbParameter> namedParameters = new List<DbParameter>();
            foreach (var kvp in parameters)
            {
                namedParameters.Add(GetNamedParameter(kvp));
            }
            return namedParameters;
        }

        protected void FillNamedParameters(DbCommand command, KeyValuePair<string, object>[] parameters)
        {
            foreach (var kvp in parameters)
            {
                command.Parameters.Add(GetNamedParameter(kvp));
            }
        }

        protected DbParameter GetNamedParameter(KeyValuePair<string, object> parameter)
        {
            var namedParameter = new SqlParameter { ParameterName = parameter.Key, Value = parameter.Value };
            return namedParameter;
        }

        protected DbCommand GetStoredProcedureCommand(string procedureName, params KeyValuePair<string, object>[] parameters)
        {
            return GetCommand(procedureName, System.Data.CommandType.StoredProcedure, parameters);
        }

        protected DbCommand GetCommand(string commandText, System.Data.CommandType type, params KeyValuePair<string, object>[] parameters)
        {
            var command = context.Database.Connection.CreateCommand();
            command.CommandText = GetCommandText(commandText, type, parameters);
            command.CommandType = type;
            if (parameters != null && parameters.Length > 0)
            {
                FillNamedParameters(command, parameters);
            }
            return command;
        }

        protected string GetCommandText(string commandText, System.Data.CommandType type, params KeyValuePair<string, object>[] parameters)
        {
            var text = new StringBuilder(string.Format("{0}{1}", type == System.Data.CommandType.StoredProcedure ? "EXEC " : string.Empty, commandText));
            if (type == System.Data.CommandType.StoredProcedure && parameters != null && parameters.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    text.AppendFormat(" @{0},", parameter.Key);
                }
                return text.Remove(text.Length - 1, 1).ToString();
            }
            return text.ToString();
        }
        #endregion


        public bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQuery(filter).Any();
        }


        public virtual void Detach(TEntity entity)
        {
            if (entity != null && Context.Entry<TEntity>(entity).State != EntityState.Detached)
            {
                Context.Entry<TEntity>(entity).State = EntityState.Detached;
            }
        }

        public virtual void Attach(TEntity entity)
        {
            if (entity != null && this.Context.Entry<TEntity>(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
        }

        private string[] GetKeyNames()
        {
            var objectSet = ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<TEntity>();
            string[] keyNames = objectSet.EntitySet.ElementType.KeyMembers.Select(k => k.Name).ToArray();
            return keyNames;
        }

        private object[] GetPrimaryKeys(TEntity entity)
        {
            var keyNames = GetKeyNames();
            Type type = typeof(TEntity);

            var keys = new object[keyNames.Length];
            for (int i = 0; i < keyNames.Length; i++)
            {
                keys[i] = type.GetProperty(keyNames[i]).GetValue(entity, null);
            }
            return keys;
        }

    }
}
