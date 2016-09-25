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

        private ICollection<Revision> _userRevisionList;
        public ICollection<Revision> UserRevisionList
        {
            get
            {
                return (_userRevisionList);
            }
            set
            {
                _userRevisionList = value;
                OnPropertyChanged();
            }
        }

        public RevisionReportVM(User user)
        {
            this.user = user;
            revisionBusiness = new RevisionBusiness();
            Task.Run(() =>
            {
                RevisionList = revisionBusiness.GetAllByIQueryable().ToList();
            });
            this._userRevisionList = user.Revisions;
        }

    }
}
