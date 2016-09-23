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

        public ICollection<User> GetAll()
        {
            return unitOfWork.UserRepository.GetList();
        }

        public IQueryable<User> GetAllUserByIQueryable()
        {
            return (unitOfWork.UserRepository.GetQuery());
        }

        public IEnumerable<User> GetUserByAccountPasswordByIQueryable(string account, string password)
        {
            return (GetAllUserByIQueryable().Where(u => u.AccountName == account && u.Password == password).ToList());
        }
        
    }
}
