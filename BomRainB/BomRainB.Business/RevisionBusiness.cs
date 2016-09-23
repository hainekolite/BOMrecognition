using BomRainB.Data;
using BomRainB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.Business
{
    public class RevisionBusiness
    {
        UnitOfWork unitOfWork;
    
        public RevisionBusiness()
        {
            unitOfWork = new UnitOfWork();
        }

        public ICollection<Revision> GetAll()
        {
            return unitOfWork.RevisionRepository.GetList();
        }

    }
}
