using BomRainB.Data.Contracts;
using BomRainB.Data.Repositories;
using BomRainB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.Data
{
    [NotMapped]
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        public BomRainBirdDbContext Context { get; set; }

        private bool disposed = false;

        #region Repositories

        public IRepository<User> UserRepository { get; set; }
        public IRepository<Revision> RevisionRepository { get; set; }

        #endregion Repositories

        public UnitOfWork()
        {
            Context = new BomRainBirdDbContext();
            this.Context.Database.CommandTimeout = 0;
            InitializeRepositories(this.Context);
        }

        private void InitializeRepositories(BomRainBirdDbContext context)
        {
            UserRepository = new UserRepository(context) as IRepository<User>;
            RevisionRepository = new RevisionRepository(context) as IRepository<Revision>;
        }

        public void CommitChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

    }
}
