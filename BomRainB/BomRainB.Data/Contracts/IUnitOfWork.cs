using BomRainB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.Data.Contracts
{
    public interface IUnitOfWork
    {
        IRepository<User> UserRepository { get; set; }
        IRepository<Revision> RevisionRepository { get; set; }

        void CommitChanges();
        void Dispose();
    }
}
