using BomRainB.ModelHelpers;
using BomRainB.ViewModel.Commands;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ViewModel
{
    public class DiferencesViewerVM : ViewModelBase
    {

        private readonly string resultAOIMessage;
        public string ResultAOIMessage => resultAOIMessage;


        private readonly string resultBOMMessage;
        public string ResultBOMMessage => resultBOMMessage;

        private readonly string title;
        public string Title => title;

        private readonly RelayCommand _acceptDialogCommand;
        public RelayCommand AcceptDialogCommand => _acceptDialogCommand;

        private ICollection<AoiInterestData> aoiCheckList;
        public ICollection<AoiInterestData> AoiCheckList
        {
            get
            {
                return (aoiCheckList);
            }
            set
            {
                aoiCheckList = value;
                OnPropertyChanged();
            }
        }

        private ICollection<BomInterestData> bomCheckList;
        public ICollection<BomInterestData> BomCheckList
        {
            get
            {
                return (bomCheckList);
            }
            set
            {
                bomCheckList = value;
                OnPropertyChanged();
            }
        }

        public DiferencesViewerVM(ICollection<BomInterestData> bomCheckList, ICollection<AoiInterestData> aoiCheckList)
        {
            resultAOIMessage = "The following parts - references from AOI file\n\rAre not present in the BOM file";
            resultBOMMessage = "The following components - references from the BOM file\n\rAre not present in the AOI file";
            title = "Check Results";
            _acceptDialogCommand = new RelayCommand(AcceptAction);

            this.bomCheckList = bomCheckList;
            this.aoiCheckList = aoiCheckList;
        }

        private void AcceptAction() => DialogHost.CloseDialogCommand.Execute(true, null);

    }

}
