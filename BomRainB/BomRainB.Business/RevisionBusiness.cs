﻿using BomRainB.Data;
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

        public IQueryable<Revision> GetAllByIQueryable()
        {
            return (unitOfWork.RevisionRepository.GetQuery());
        }

        public void InsertRevision(User user, string documentName, string revisionVersion)
        {
            Revision rev = new Revision()
            {
                DocumentName = documentName,
                DocuemntVersion = revisionVersion,
                Date = DateTime.Now,
                UserId = user.Id
            };
            unitOfWork.RevisionRepository.Insert(rev);
            unitOfWork.CommitChanges();
        }
    }
}
