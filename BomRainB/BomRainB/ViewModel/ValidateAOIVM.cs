using BomRainB.ViewModel.Commands;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ViewModel
{
    public class ValidateAOIVM : ViewModelBase
    {
        private readonly string title;
        public string Title => title;

        private readonly string dialogContent;
        public string DialogContent => dialogContent;

        private readonly RelayCommand _acceptActionCommand;
        public RelayCommand AcceptActionCommand => _acceptActionCommand;

        private readonly RelayCommand _cancelActionCommand;
        public RelayCommand CancelActionCommand => _cancelActionCommand;

        public ValidateAOIVM()
        {
            title = "Validate AOI";
            dialogContent = "Validation will be executed.\n\r Are you Sure?";
            _acceptActionCommand = new RelayCommand(AcceptAction);
            _cancelActionCommand = new RelayCommand(CancelAction);
        }

        private void AcceptAction() => DialogHost.CloseDialogCommand.Execute(true, null);

        private void CancelAction() => DialogHost.CloseDialogCommand.Execute(false, null);

    }
}
