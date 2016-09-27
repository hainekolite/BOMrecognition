using BomRainB.Data;
using BomRainB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.Business
{
    public class UserBusiness
    {
        UnitOfWork unitOfWork;

        public UserBusiness()
        {
            unitOfWork = new UnitOfWork();
        }

        public ICollection<User> GetAll() => (unitOfWork.UserRepository.GetList());

        public IQueryable<User> GetAllUserByIQueryable() => 
            (unitOfWork.UserRepository.GetQuery(null,null, includeProperties : GetIncludeProperties()));

        public IEnumerable<User> GetUserByAccountPasswordByIQueryable(string account, string password) => 
            (GetAllUserByIQueryable().Where(u => u.AccountName == account && u.Password == password).ToList());
        
        public User GetUserById(int userId) => (unitOfWork.UserRepository.GetByID(userId));
        
        public string[] GetIncludeProperties() => new[] {"Revisions"};

    }
}
