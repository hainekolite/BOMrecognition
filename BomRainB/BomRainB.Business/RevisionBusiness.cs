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

        public ICollection<Revision> GetAll() => unitOfWork.RevisionRepository.GetList();

        public IQueryable<Revision> GetAllByIQueryable() => (unitOfWork.RevisionRepository.GetQuery());

        public IQueryable<Revision> GetAllByIQueryableOrderedDescending() => (GetAllByIQueryable().OrderByDescending(r => r.Date));

        public IQueryable<Revision> GetRevisionByDocumentName(string documentName) => (unitOfWork.RevisionRepository.GetQuery().Where(r=> r.DocumentName == documentName));

        public IQueryable<Revision> GetAllRevisionsInDateRange(DateTime beginDate, DateTime finishDate) =>
            GetAllByIQueryable().Where(r => r.Date >= beginDate && r.Date <= finishDate).OrderByDescending(r => r.Date);

        public void InsertRevision(User user, string documentName, string revisionVersion)
        {
            Revision rev = new Revision()
            {
                DocumentName = documentName,
                DocuemntVersion = revisionVersion,
                Date = DateTime.Now,
                UserId = user.Id
            };
            user.Revisions.Add(rev);
            unitOfWork.RevisionRepository.Insert(rev);
            unitOfWork.CommitChanges();
        }
    }
}
