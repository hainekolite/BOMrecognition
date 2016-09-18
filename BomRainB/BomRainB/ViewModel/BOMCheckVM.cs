using BomRainB.ModelHelpers;
using BomRainB.ViewModel.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace BomRainB.ViewModel
{
    public class BOMCheckVM : ViewModelBase
    {
        private OpenFileDialog documentTxt;
        private OpenFileDialog documentCsv;
        private const string DIALOG_TXT_TITLE = "Select the AOL document in .txt format";
        private const string DIALOG_CSV_TITLE = "Select the CSV document in .csv format";
        private const string NO_FILES_SELECTED = "Press the open button to Select a file";
        private const string FILE_NOT_AVIALABLE = "The file is being used by another process. The file path specified doesn't longer exist or the file has been deleted";
        private const string COMPONENET_ID_REFERENCE_NOT_PRESENT = "The file does not contain a header with the legend Componenet ID or Reference Designator please check the file before the load";
        private const string ERROR = "ERROR";
        private const char QUOTE_MARK_REPLACEMENT = '\'';
        private const char QUOTE_MARK = '"';
        private const char COLON = ',';

        private string selectedFileTXT;
        private static string[] BomInterestHeaders = {"COMPONENTID", "REFERENCEDESIGNATORS"};
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

        public IEnumerable<BomInterestData> _bomCSVList;
        public IEnumerable<BomInterestData> BomCSVList
        {
            get
            {
                return (_bomCSVList);
            }
            set
            {
                _bomCSVList = value;
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
            }
        }

        private void GetCsvFile()
        {
            bool isFileAvailable = true;
            string[] rawCSVdata;
            documentCsv.ShowDialog();
            if (!(string.IsNullOrEmpty(documentCsv.FileName)))
            {
                rawCSVdata = ReadCSVFile(documentCsv.FileName, out isFileAvailable);
                if (isFileAvailable)
                {                     
                    BomCSVList = (GetBomInterestData(rawCSVdata));
                    if (BomCSVList != null)
                        SelectedFileCSV = documentCsv.SafeFileName;
                    else
                        SelectedFileCSV = NO_FILES_SELECTED;
                }
            }
        }

        private List<BomInterestData> GetBomInterestData(string[] csvRawData)
        {
            int[] offset = { -1, -1 };
            bool flag = true;
            bool isHeaderPresent = true;
            string[] filteredCSVData = GetCSVFilteredData(csvRawData, out isHeaderPresent);
            if (isHeaderPresent)
            {
                return filteredCSVData.Select(line =>
                {
                    string[] data = line.Split(COLON);
                    ReturnColons(data);
                    if (flag)
                        GetOffset(data, offset, out flag);
                    return (new BomInterestData(data[offset[0]], data[offset[1]]));
                }).ToList();
            }
            MessageBox.Show(COMPONENET_ID_REFERENCE_NOT_PRESENT, ERROR);
            return (null);
        }

        private string[] ReturnColons(string[] lines)
        {
            for(int i=0; i<lines.Count(); i++)
            {
                if (lines[i].Contains(QUOTE_MARK_REPLACEMENT))
                    lines[i] = lines[i].Replace(QUOTE_MARK_REPLACEMENT, COLON);
            }
            return (lines);
        }

        private string[] GetCSVFilteredData(string[] csvRawData, out bool isHeaderPresent)
        {
            isHeaderPresent = true;
            for (int i = 0; i < csvRawData.Count(); i++)
            {
                if (Regex.Replace(csvRawData[i], " ", "").ToUpper().Contains(BomInterestHeaders[0]) && Regex.Replace(csvRawData[i], " ", "").ToUpper().Contains(BomInterestHeaders[1]))
                    return (ReplaceQuoteMarks(csvRawData.Skip(i).ToArray()));
            }
            isHeaderPresent = false;
            return (null);
        }

        private void GetOffset(string[] data, int[] offset, out bool flag)
        {
            int i;
            for (int j = 0; j<BomInterestHeaders.Count(); j++)
            {
                for (i = 0; i < data.Count(); i++)
                {
                    if (Regex.Replace(data[i], " ", "").ToUpper().Contains(BomInterestHeaders[j]))
                        break;
                }
                offset[j] = i;
            }
            flag = false;
        }

        private string[] ReplaceQuoteMarks(string[] csvRawData)
        {
            int quoteCount = 0;
            int[] index = { 0, 0 };
            for (int i = 0; i < csvRawData.Count(); i++)
            {
                while(csvRawData[i].Contains(QUOTE_MARK))
                {
                    for (int j = 0; j < csvRawData[i].Length; j++)
                    {
                        if (csvRawData[i][j].Equals(QUOTE_MARK))
                        {
                            quoteCount++;
                            index[quoteCount - 1] = j;
                            if (quoteCount == 2)
                            {
                                quoteCount = 0;
                                for (int z = index[0]; z < index[1]; z++)
                                {
                                    if (csvRawData[i][z].Equals(COLON))
                                    {
                                        StringBuilder sb = new StringBuilder(csvRawData[i].ToString());
                                        sb[z] = QUOTE_MARK_REPLACEMENT;
                                        csvRawData[i] = sb.ToString();
                                    }      
                                }
                                csvRawData[i] = csvRawData[i].Remove(index[1], 1);
                                csvRawData[i] = csvRawData[i].Remove(index[0], 1);
                            }
                        }
                    }
                    quoteCount = 0;
                }
            }
            return (csvRawData);
        }


        private string[] ReadCSVFile(string path, out bool isFileAvailable )
        {
            string[] fileLines;
            try
            {
                isFileAvailable = true;
                return (fileLines = File.ReadAllLines(path));
            }
            catch (Exception e)
            {
                MessageBox.Show(FILE_NOT_AVIALABLE, ERROR);
                isFileAvailable = false;
                return (new string[] { "" });
            }
        }

    }
}
