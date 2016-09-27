using BomRainB.Business;
using BomRainB.Models;
using BomRainB.ViewModel.Commands;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BomRainB.ViewModel
{
    public class RevisionReportVM : ViewModelBase
    {
        private readonly RevisionBusiness revisionBusiness;
        private readonly UserBusiness userBusiness;
        private readonly User user;

        private readonly ParameterCommand _filterByDateCommand;
        public ParameterCommand FilterByDateCommand => _filterByDateCommand;

        private DateTime _initialDate;
        public DateTime InitialDate
        {
            get
            {
                return (_initialDate);
            }
            set
            {
                _initialDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _finishDate;
        public DateTime FinishDate
        {
            get
            {
                return (_finishDate);
            }
            set
            {
                _finishDate = value;
                OnPropertyChanged();
            }
        }

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

        private ICollection<User> _revisionListUsers;
        public ICollection<User> RevisionListUsers
        {
            get
            {
                return (_revisionListUsers);
            }
            set
            {
                _revisionListUsers = value;
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

        private Task GeneraListThread;

        public RevisionReportVM(User user)
        {
            this.user = user;
            _initialDate = DateTime.Now;
            _finishDate = DateTime.Now;
            revisionBusiness = new RevisionBusiness();
            GeneraListThread = Task.Run(() =>
            {
                RevisionList = revisionBusiness.GetAllByIQueryableOrderedDescending().ToList();
            });
            this._userRevisionList = user.Revisions.OrderByDescending(r => r.Date).ToList();
            _filterByDateCommand = new ParameterCommand(FilterRevisionsByDate);
        }

        private void FilterRevisionsByDate(object datePickers)
        {
            var values = datePickers as object[];
            DatePicker firstDate = values[0] as DatePicker;
            DatePicker lastDate = values[1] as DatePicker;
            if (firstDate.SelectedDate >= lastDate.SelectedDate)
                MessageBox.Show("Initial date can not be higher than finish date, also dates cannot be the same","ERROR");
            else
            {
                if (RevisionList != null && GeneraListThread.IsCompleted)
                {
                    _initialDate = ((DateTime)firstDate.SelectedDate);
                    _finishDate = ((DateTime)lastDate.SelectedDate);
                    GeneraListThread = Task.Run(() => {
                        RevisionList = revisionBusiness.GetAllRevisionsInDateRange(_initialDate, _finishDate).ToList();
                    });
                    
                }
                else
                    MessageBox.Show("the revision list still loading data","ERROR");
            }

        }

    }
}
