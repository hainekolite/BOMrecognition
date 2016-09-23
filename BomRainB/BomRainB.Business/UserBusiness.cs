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
        public UnitOfWork unitOfWork;

        public UserBusiness()
        {
            unitOfWork = new UnitOfWork();
        }

        public ICollection<User> GetAll()
        {
            return unitOfWork.UserRepository.GetList();
        }
    }
}
