using BomRainB.Business;
using BomRainB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ViewModel
{
    public class RevisionReportVM : ViewModelBase
    {
        private readonly RevisionBusiness revisionBusiness;
        private readonly User user;

        private ICollection<Revision> _revisionList;
        public ICollection<Revision> RevisionList
        {
            get
            {
                return (_revisionList);
            }
            set
            {
                _revisionList = value;
                OnPropertyChanged();
            }
        }

        public RevisionReportVM(User user)
        {
            this.user = user;
            revisionBusiness = new RevisionBusiness();
            RevisionList = revisionBusiness.GetAllByIQueryable().ToList();
        }

    }
}
