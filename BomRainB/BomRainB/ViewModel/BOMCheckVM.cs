using BomRainB.ViewModel.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.ViewModel
{
    public class BOMCheckVM : ViewModelBase
    {
        private OpenFileDialog documentTxt;
        private OpenFileDialog documentCsv;
        private const string DIALOG_TXT_TITLE = "Select the AOL document in txt format";
        private const string DIALOG_CSV_TITLE = "Select the CSV document in txt format";
        private const string NO_FILES_SELECTED = "Press the open button to Select a file";
        private string selectedFileTXT;
        public string SelectedFileTXT
        {
            get
            {
                return (selectedFileTXT);
            }
            set
            {
                selectedFileTXT = value;
                OnPropertyChanged();
            }
        }
        private string selectedFileCSV;
        public string SelectedFileCSV
        {
            get
            {
                return (selectedFileCSV);
            }
            set
            {
                selectedFileCSV = value;
                OnPropertyChanged();
            }
        }

        private readonly RelayCommand _selectTxtFileDialogCommand;
        public RelayCommand SelectTxtFileDialogCommand => _selectTxtFileDialogCommand;

        private readonly RelayCommand _selectCsvFileDialogCommand;
        public RelayCommand SelectCsvFileDialogCommand => _selectCsvFileDialogCommand;

        public BOMCheckVM()
        {
            documentTxt = new OpenFileDialog();
            documentTxt.Multiselect = false;
            documentTxt.Filter = "Text only|*.txt;";
            documentTxt.Title = DIALOG_TXT_TITLE;

            documentCsv = new OpenFileDialog();
            documentCsv.Multiselect = false;
            documentCsv.Filter = "Csv only|*.csv;";
            documentCsv.Title = DIALOG_CSV_TITLE;

            selectedFileTXT = NO_FILES_SELECTED;
            selectedFileCSV = NO_FILES_SELECTED;

            _selectTxtFileDialogCommand = new RelayCommand(GetTxtFile);
            _selectCsvFileDialogCommand = new RelayCommand(GetCsvFile);
        }

        private void GetTxtFile()
        {
            documentTxt.ShowDialog();
            if (!(string.IsNullOrEmpty(documentTxt.FileName)))
            {
                SelectedFileTXT = documentTxt.SafeFileName;
                //selectedFileTXT = documentTxt.FileName; retorna tolda la ruta
                //render for the DataGrid
            }
        }

        private void GetCsvFile()
        {
            documentCsv.ShowDialog();
            if (!(string.IsNullOrEmpty(documentCsv.FileName)))
            {
                SelectedFileCSV = documentCsv.SafeFileName;
                //selectedFileTXT = documentTxt.FileName; retorna tolda la ruta
                //render for the DataGrid
            }
        }

    }
}
