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
        #region Properties
        private OpenFileDialog documentTxt;
        private OpenFileDialog documentCsv;
        private const string DIALOG_TXT_TITLE = "Select the AOI document in .txt format";
        private const string DIALOG_CSV_TITLE = "Select the BOM document in .csv format";
        private const string NO_FILES_SELECTED = "Press the open button to Select a file";
        private const string FILE_NOT_AVIALABLE = "The file is being used by another process. The file path specified doesn't longer exist or the file has been deleted";
        private const string COMPONENET_ID_REFERENCE_NOT_PRESENT = "The file does not contain a header with the legend Componenet ID or Reference Designators please check the file before the load";
        private const string PART_NUMBER_REFERENCE_NOT_PRESENT = "The file does not contain a header with the legend Part Number or Reference Designator please check the file before the load";
        private const string ERROR = "ERROR";
        private const char QUOTE_MARK_REPLACEMENT = '\'';
        private const char QUOTE_MARK = '"';
        private const char COLON = ',';

        private Task BomThread;
        private Task AoiThread;

        private string selectedFileTXT;

        private static string[] BomInterestHeaders = { "COMPONENTID", "REFERENCEDESIGNATORS" };
        private static string[] AoiInterestHeaders = { "PARTNUMBER", "REFERENCEDESIGNATOR" };

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

        private IEnumerable<BomInterestData> _bomCSVList;
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

        private IEnumerable<AoiInterestData> _aoiTXTList;
        public IEnumerable<AoiInterestData> AoiTXTList
        {
            get
            {
                return (_aoiTXTList);
            }
            set
            {
                _aoiTXTList = value;
                OnPropertyChanged();
            }
        }

        private readonly RelayCommand _selectTxtFileDialogCommand;
        public RelayCommand SelectTxtFileDialogCommand => _selectTxtFileDialogCommand;

        private readonly RelayCommand _selectCsvFileDialogCommand;
        public RelayCommand SelectCsvFileDialogCommand => _selectCsvFileDialogCommand;
        #endregion Properties

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
            if (AoiThread != null)
            {
                if (AoiThread.IsCompleted)
                {
                    documentTxt.ShowDialog();
                    if (!(string.IsNullOrEmpty(documentTxt.FileName)))
                        AoiThread = ProcessTxtFile();
                }
            }
            else
            {
                documentTxt.ShowDialog();
                if (!(string.IsNullOrEmpty(documentTxt.FileName)))
                    AoiThread = ProcessTxtFile();
            }
        }            

        private void GetCsvFile()
        {
            if (BomThread != null)
            {
                if (BomThread.IsCompleted)
                {
                    documentCsv.ShowDialog();
                    if (!(string.IsNullOrEmpty(documentCsv.FileName)))
                        BomThread = ProcessCsvFile();
                }        
            }
            else
            {
                documentCsv.ShowDialog();
                if (!(string.IsNullOrEmpty(documentCsv.FileName)))
                    BomThread = ProcessCsvFile();
            }
        }

        #region Task

        private Task ProcessCsvFile()
        {
            return (Task.Run(() =>
             {
                 bool isFileAvailable = true;
                 string[] rawCsvData;
                 rawCsvData = ReadFile(documentCsv.FileName, out isFileAvailable);
                 if (isFileAvailable)
                 {
                     BomCSVList = GetBomInterestData(rawCsvData);
                     if (BomCSVList != null)
                         SelectedFileCSV = documentCsv.SafeFileName;
                     else
                         SelectedFileCSV = NO_FILES_SELECTED;
                 }
                 documentCsv.FileName = null;
             }));
        }

        private Task ProcessTxtFile()
        {
            return (Task.Run(() =>
             {
                 bool isFileAvailable = true;
                 string[] rawTxtData;
                 rawTxtData = ReadFile(documentTxt.FileName, out isFileAvailable);
                 if (isFileAvailable)
                 {
                     AoiTXTList = GetAoiInterestData(rawTxtData);
                     if (AoiTXTList != null)
                         SelectedFileTXT = documentTxt.SafeFileName;
                     else
                         SelectedFileCSV = NO_FILES_SELECTED;
                 }
                 documentTxt.FileName = null;
             }));
        }

        #endregion Task

        #region AOI-TXTRelated
        private List<AoiInterestData> GetAoiInterestData(string[] rawTxtData)
        {
            int[] offset = { -1, -1 };
            bool flag = true;
            bool isHeaderPresent = true;
            string[] filteredTXTVData = GetFilteredData(rawTxtData, out isHeaderPresent, AoiInterestHeaders);
            if (isHeaderPresent)
            {
                return filteredTXTVData.Select(line =>
                {
                    string[] data = line.Split(COLON);
                    ReturnColons(data);
                    if (flag)
                        GetHeaderOffset(data, offset, out flag, AoiInterestHeaders);
                    return (new AoiInterestData(data[offset[0]], data[offset[1]]));
                }).ToList();
            }
            MessageBox.Show(PART_NUMBER_REFERENCE_NOT_PRESENT, ERROR);
            return (null);
        }

        #endregion AOI-TXTRelated

        #region BOM-CSVRelated

        private List<BomInterestData> GetBomInterestData(string[] csvRawData)
        {
            int[] offset = { -1, -1 };
            bool flag = true;
            bool isHeaderPresent = true;
            string[] filteredCSVData = GetFilteredData(csvRawData, out isHeaderPresent, BomInterestHeaders);
            if (isHeaderPresent)
            {
                return filteredCSVData.Select(line =>
                {
                    string[] data = line.Split(COLON);
                    ReturnColons(data);
                    if (flag)
                        GetHeaderOffset(data, offset, out flag, BomInterestHeaders);
                    return (new BomInterestData(data[offset[0]], data[offset[1]]));
                }).ToList();
            }
            MessageBox.Show(COMPONENET_ID_REFERENCE_NOT_PRESENT, ERROR);
            return (null);
        }
        #endregion BOM-CSVRelated

        #region GeneralUse

        private string[] ReadFile(string path, out bool isFileAvailable )
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
                return (null);
            }
        }

        private string[] GetFilteredData(string[] rawData, out bool isHeaderPresent, string[] headers)
        {
            isHeaderPresent = true;
            for (int i = 0; i < rawData.Count(); i++)
            {
                if (Regex.Replace(rawData[i], " ", "").ToUpper().Contains(headers[0]) && Regex.Replace(rawData[i], " ", "").ToUpper().Contains(headers[1]))
                    return (ReplaceQuoteMarks(rawData.Skip(i).ToArray()));
            }
            isHeaderPresent = false;
            return (null);
        }

        private void GetHeaderOffset(string[] data, int[] offset, out bool flag, string[] headers)
        {
            int i;
            for (int j = 0; j < headers.Count(); j++)
            {
                for (i = 0; i < data.Count(); i++)
                {
                    if (Regex.Replace(data[i], " ", "").ToUpper().Contains(headers[j]))
                        break;
                }
                offset[j] = i;
            }
            flag = false;
        }

        private string[] ReplaceQuoteMarks(string[] rawData)
        {
            int quoteCount = 0;
            int[] index = { 0, 0 };
            for (int i = 0; i < rawData.Count(); i++)
            {
                while (rawData[i].Contains(QUOTE_MARK))
                {
                    for (int j = 0; j < rawData[i].Length; j++)
                    {
                        if (rawData[i][j].Equals(QUOTE_MARK))
                        {
                            quoteCount++;
                            index[quoteCount - 1] = j;
                            if (quoteCount == 2)
                            {
                                quoteCount = 0;
                                for (int z = index[0]; z < index[1]; z++)
                                {
                                    if (rawData[i][z].Equals(COLON))
                                    {
                                        StringBuilder sb = new StringBuilder(rawData[i].ToString());
                                        sb[z] = QUOTE_MARK_REPLACEMENT;
                                        rawData[i] = sb.ToString();
                                    }
                                }
                                rawData[i] = rawData[i].Remove(index[1], 1);
                                rawData[i] = rawData[i].Remove(index[0], 1);
                            }
                        }
                    }
                    quoteCount = 0;
                }
            }
            return (rawData);
        }

        private void ReturnColons(string[] lines)
        {
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Contains(QUOTE_MARK_REPLACEMENT))
                    lines[i] = lines[i].Replace(QUOTE_MARK_REPLACEMENT, COLON);
            }            
        }

        #endregion GeneralUse

    }
}
