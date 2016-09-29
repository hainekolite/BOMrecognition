using BomRainB.Business;
using BomRainB.ModelHelpers;
using BomRainB.Models;
using BomRainB.ViewModel.Commands;
using BomRainB.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
        private const string BOM_DIRECTORY_PATH = "BomDirectoryPath";
        private const string AOI_DIRECTORY_PATH = "AoiDirectoryPath";
        private const string DIALOG_TXT_TITLE = "Select the AOI document in .txt format";
        private const string DIALOG_CSV_TITLE = "Select the BOM document in .csv format";
        private const string NO_FILES_SELECTED = "Press the open button to Select a file";
        private const string FILE_NOT_AVIALABLE = "The file is being used by another process. The file path specified does not longer exist or the file has been deleted";
        private const string COMPONENET_ID_REFERENCE_NOT_PRESENT = "The file does not contain a header with the legend Rev, Component ID or Reference Designators, check all the fields mentioned before.\n\rPlease check the file before the it";
        private const string PART_NUMBER_REFERENCE_NOT_PRESENT = "The file does not contain a header with the legend Part Number or Reference Designator please check the file before the load";
        private const string DOT_EXTENSION_NOT_PRESENT = "Something went wrong, one or more files have not been loaded or one of your files does not contain the correct file extension .txt or .csv. Please check the files before start the check";
        private const string FILE_NAMES_NOT_EQUAL = "Files names does not match each other. Please select the correct files to start the check process";
        private const string FILE_ONLY_HAVE_HEADERS = "Something went wrong, the file only contains the headers, no data was identified. Check your file please";
        private const string REVISION_ADDED = "Revision Added Successfully";
        private const string REVISION_OUTDATED = "There is a same or newer version of your document in the database, please check the versioning of the document in the report section";
        private const string ERROR = "ERROR";
        private const string SUCCESS = "SUCCESS";
        private const char QUOTE_MARK_REPLACEMENT = '\'';
        private const char QUOTE_MARK = '"';
        private const char COLON = ',';
        private const string REVISION_QUOTE = "REV";

        private Task BomThread;
        private Task AoiThread;
        private Object bomLock;
        private Object aoiLock;

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

        private string selectedRevisionVersion { get; set; }

        private ICollection<BomInterestData> _memBomCSVList;
        private ICollection<BomInterestData> _bomCSVList;
        public ICollection<BomInterestData> BomCSVList
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

        private ICollection<AoiInterestData> _memAoiTXTList;
        private ICollection<AoiInterestData> _aoiTXTList;
        public ICollection<AoiInterestData> AoiTXTList
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

        private readonly RelayCommand _checkFilesCommand;
        public RelayCommand CheckFilesCommand => _checkFilesCommand;

        private readonly RelayCommand _validateAOIfileCommand;
        public RelayCommand ValidateAOIfileCommand => _validateAOIfileCommand;

        private readonly User user;
        private readonly RevisionBusiness revisionBusiness;

        #endregion Properties

        #region Constructor
        public BOMCheckVM(User mainWindowUser)
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
            selectedRevisionVersion = string.Empty;

            _selectTxtFileDialogCommand = new RelayCommand(GetTxtFile);
            _selectCsvFileDialogCommand = new RelayCommand(GetCsvFile);
            _checkFilesCommand = new RelayCommand(CheckBomVsAoi);
            _validateAOIfileCommand = new RelayCommand(ValidateAOIFile);
            user = mainWindowUser;
            revisionBusiness = new RevisionBusiness();

            bomLock = new object();
            aoiLock = new object();

        }
        #endregion Constructor

        #region Validate-AOI-FIle

        private void ValidateAOIFile()
        {
            string _selectedFileTXT = selectedFileTXT;
            string _selectedFileCSV = selectedFileCSV;
            if (_selectedFileTXT.Contains(".") && _selectedFileCSV.Contains("."))
            {
                _selectedFileCSV = RemoveDotExtension(_selectedFileCSV);
                _selectedFileTXT = RemoveDotExtension(_selectedFileTXT);
                if (!(string.IsNullOrEmpty(_selectedFileCSV) && string.IsNullOrEmpty(_selectedFileTXT)))
                {
                    if (_selectedFileCSV.Equals(_selectedFileTXT))
                    {
                        if (_memBomCSVList != null && _memAoiTXTList != null)
                        {
                            InsertValidation();
                        }
                    }
                    else
                        MessageBox.Show(FILE_NAMES_NOT_EQUAL, ERROR);
                }
                else
                    MessageBox.Show(DOT_EXTENSION_NOT_PRESENT, ERROR);
            }
            else
                MessageBox.Show(DOT_EXTENSION_NOT_PRESENT, ERROR);
        }

        private async void InsertValidation()
        {
            ValidateAOI dialogView = new ValidateAOI() { DataContext = new ValidateAOIVM() };
            var result = (bool)(await(DialogHost.Show(dialogView, "RootDialog")));
            string fileName = RemoveDotExtension(selectedFileCSV);
            selectedRevisionVersion = Regex.Replace(selectedRevisionVersion," ",string.Empty); 
            if (result)
            {
                //new Task(() => {
                var query = revisionBusiness.GetRevisionByDocumentName(fileName)?.ToList();//.Last();
                if (query != null && query.Count >= 1)
                {
                    if (IsRevisionVersionValid(selectedRevisionVersion, query.OrderByDescending(r => r.Date).FirstOrDefault().DocumentVersion))
                    {
                        revisionBusiness.InsertRevision(this.user, fileName, selectedRevisionVersion.Trim().ToUpper());
                        MessageBox.Show(REVISION_ADDED, SUCCESS);
                    }
                    else
                        MessageBox.Show(REVISION_OUTDATED, ERROR);
                }
                else
                {
                revisionBusiness.InsertRevision(this.user, fileName, selectedRevisionVersion.Trim().ToUpper());
                MessageBox.Show(REVISION_ADDED, SUCCESS);
                }
                //});
            }
        }
        #endregion Validate-AOI-FIle

        #region CheckRegion

        private void CheckBomVsAoi()
        {
            string _selectedFileTXT = selectedFileTXT;
            string _selectedFileCSV = selectedFileCSV;
            if (_selectedFileTXT.Contains(".") && _selectedFileCSV.Contains("."))
            {
                _selectedFileCSV = RemoveDotExtension(_selectedFileCSV);
                _selectedFileTXT = RemoveDotExtension(_selectedFileTXT);
                if (!(string.IsNullOrEmpty(_selectedFileCSV) && string.IsNullOrEmpty(_selectedFileTXT)))
                {
                    if (_selectedFileCSV.Equals(_selectedFileTXT))
                    {
                        if (_memBomCSVList != null && _memAoiTXTList != null)
                        {
                            PerformCheck(_memBomCSVList, _memAoiTXTList);
                        }
                    }
                    else
                        MessageBox.Show(FILE_NAMES_NOT_EQUAL, ERROR);
                }
                else
                    MessageBox.Show(DOT_EXTENSION_NOT_PRESENT, ERROR);
            }
            else
                MessageBox.Show(DOT_EXTENSION_NOT_PRESENT, ERROR);
        }

        private async void PerformCheck(ICollection<BomInterestData> _memBomCSVList, ICollection<AoiInterestData> _memAoiTXTList)
        {
            var aoiTempList = _memAoiTXTList.Select(y => new { y.partNumber, y.referenceDesignator }).Distinct().ToList();
            var bomTempList = _memBomCSVList.Select(x => new { x.componentId, x.referenceDesignator }).Distinct().ToList();
            ICollection<AoiInterestData> aoiCheckList = new List<AoiInterestData>();
            ICollection<BomInterestData> bomCheckList = new List<BomInterestData>();

            for (int i = 0; i < aoiTempList.Count; i++)
            {
                if (!(bomTempList.Contains(new { componentId = aoiTempList.ElementAt(i).partNumber, referenceDesignator = aoiTempList.ElementAt(i).referenceDesignator })))
                    aoiCheckList.Add(new AoiInterestData(aoiTempList.ElementAt(i).partNumber, aoiTempList.ElementAt(i).referenceDesignator));
            }
            for (int i = 0; i < bomTempList.Count; i++)
            {
                if (!(aoiTempList.Contains(new { partNumber = bomTempList.ElementAt(i).componentId, referenceDesignator = bomTempList.ElementAt(i).referenceDesignator })))
                    bomCheckList.Add(new BomInterestData(bomTempList.ElementAt(i).componentId, bomTempList.ElementAt(i).referenceDesignator));
            }

            DiferencesViewer dialogView = new DiferencesViewer() { DataContext = new DiferencesViewerVM(bomCheckList, aoiCheckList) };
            await (DialogHost.Show(dialogView, "RootDialog"));
        }

        #endregion CheckRegion 

        #region Task

        private Task ProcessCsvFile()
        {
            return (Task.Run(() =>
             {
                 bool isFileAvailable = true;
                 string[] rawCsvData;
                 lock (bomLock)
                 {
                     rawCsvData = ReadFile(documentCsv.FileName, out isFileAvailable);
                     if (isFileAvailable)
                     {
                         lock (bomLock)
                         {
                             BomCSVList = GetBomInterestData(rawCsvData);
                             if (BomCSVList != null)
                             {
                                 DeleteHeaders(BomCSVList);
                                 DeleteSpaces(BomCSVList);
                                 DeleteBlankRowsCSV(BomCSVList);
                                 if (BomCSVList.Count > 1)
                                 {
                                     SelectedFileCSV = documentCsv.SafeFileName;
                                     BomCSVList = BomCSVList.Skip(1).ToList();
                                     _memBomCSVList = new List<BomInterestData>(BomCSVList);
                                     SpecialRanges(_memBomCSVList);
                                     SplitReferences(_memBomCSVList);
                                 }
                                 else
                                 {
                                     SelectedFileCSV = NO_FILES_SELECTED;
                                     BomCSVList.Clear();
                                     _memBomCSVList = null;
                                     selectedRevisionVersion = string.Empty;
                                     MessageBox.Show(FILE_ONLY_HAVE_HEADERS, ERROR);
                                 }
                             }
                             else
                                 SelectedFileCSV = NO_FILES_SELECTED;
                         }
                     }
                     documentCsv.FileName = null;
                 }
             }));
        }

        private Task ProcessTxtFile()
        {
            return (Task.Run(() =>
             {
                 bool isFileAvailable = true;
                 string[] rawTxtData;
                 lock (aoiLock)
                 {
                     rawTxtData = ReadFile(documentTxt.FileName, out isFileAvailable);
                     if (isFileAvailable)
                     {
                         AoiTXTList = GetAoiInterestData(rawTxtData);
                         if (AoiTXTList != null)
                         {
                             SelectedFileTXT = documentTxt.SafeFileName;
                             DeleteHeaders(AoiTXTList);
                             DeleteSpaces(AoiTXTList);
                             DelteBlankRowsTXT(AoiTXTList);
                             if (AoiTXTList.Count > 1)
                             {
                                 AoiTXTList = AoiTXTList.Skip(1).ToList();
                                 _memAoiTXTList = new List<AoiInterestData>(AoiTXTList);
                             }
                             else
                             {
                                 AoiTXTList.Clear();
                                 _memAoiTXTList = null;
                                 SelectedFileTXT = NO_FILES_SELECTED;
                                 MessageBox.Show(FILE_ONLY_HAVE_HEADERS, ERROR);
                             }
                         }
                         else
                             SelectedFileCSV = NO_FILES_SELECTED;
                     }
                     documentTxt.FileName = null;
                 }
             }));
        }

        #endregion Task

        #region AOI-TXTRelated

        private void GetTxtFile()
        {
            if (AoiThread != null)
            {
                if (AoiThread.IsCompleted)
                {
                    if (Directory.Exists(ConfigurationManager.AppSettings.Get(AOI_DIRECTORY_PATH)))
                        documentTxt.InitialDirectory = ConfigurationManager.AppSettings.Get(AOI_DIRECTORY_PATH);
                    else
                    {
                        if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal))))
                            documentTxt.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                    }
                    documentTxt.ShowDialog();
                    if (!(string.IsNullOrEmpty(documentTxt.FileName)))
                        AoiThread = ProcessTxtFile();
                }
            }
            else
            {
                string data = ConfigurationManager.AppSettings.Get(AOI_DIRECTORY_PATH);
                if (Directory.Exists(ConfigurationManager.AppSettings.Get(AOI_DIRECTORY_PATH)))
                    documentTxt.InitialDirectory = ConfigurationManager.AppSettings.Get(AOI_DIRECTORY_PATH);
                else
                {
                    if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal))))
                        documentTxt.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                }
                documentTxt.ShowDialog();
                if (!(string.IsNullOrEmpty(documentTxt.FileName)))
                    AoiThread = ProcessTxtFile();
            }
        }

        private List<AoiInterestData> GetAoiInterestData(string[] rawTxtData)
        {
            int[] offset = { -1, -1 };
            int dump = 0;
            bool flag = true;
            bool isHeaderPresent = true;
            string[] filteredTXTVData = GetFilteredData(rawTxtData, out isHeaderPresent, AoiInterestHeaders, out dump);
            if (isHeaderPresent)
            {
                return filteredTXTVData.Select(line =>
                {
                    string[] data = line.Split(COLON);
                    ReturnColons(data);
                    if (flag)
                        GetHeaderOffset(data, offset, out flag, AoiInterestHeaders);
                    try
                    {
                        if (data?.Count() > offset[1] && data?.Count() > offset[0])
                        {
                            return (new AoiInterestData(data[offset[0]], data[offset[1]]));
                        }
                        else
                        {
                            return (new AoiInterestData(" ", " "));
                        }
                    }
                    catch(Exception e)
                    {
                        return (new AoiInterestData(" ", " "));
                    }
                }).ToList();
            }
            MessageBox.Show(PART_NUMBER_REFERENCE_NOT_PRESENT, ERROR);
            _memBomCSVList = null;
            return (null);
        }

        #endregion AOI-TXTRelated

        #region BOM-CSVRelated

        private void GetCsvFile()
        {
            if (BomThread != null)
            {
                if (BomThread.IsCompleted)
                {
                    if (Directory.Exists(ConfigurationManager.AppSettings.Get(BOM_DIRECTORY_PATH)))
                        documentCsv.InitialDirectory = ConfigurationManager.AppSettings.Get(BOM_DIRECTORY_PATH);
                    else
                    {
                        if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal))))
                            documentCsv.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                    }
                    documentCsv.ShowDialog();
                    if (!(string.IsNullOrEmpty(documentCsv.FileName)))
                        BomThread = ProcessCsvFile();
                }
            }
            else
            {
                string data = ConfigurationManager.AppSettings.Get(BOM_DIRECTORY_PATH);
                if (Directory.Exists(ConfigurationManager.AppSettings.Get(BOM_DIRECTORY_PATH)))
                    documentCsv.InitialDirectory = ConfigurationManager.AppSettings.Get(BOM_DIRECTORY_PATH);
                else
                {
                    if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal))))
                        documentCsv.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                }                
                documentCsv.ShowDialog();
                if (!(string.IsNullOrEmpty(documentCsv.FileName)))
                    BomThread = ProcessCsvFile();
            }
        }

        private List<BomInterestData> GetBomInterestData(string[] csvRawData)
        {
            int[] offset = { -1, -1 };
            int RevisionZone = 0;
            bool flag = true;
            bool isHeaderPresent = true;
            bool isRevisionPresent = true;
            string[] filteredCSVData = GetFilteredData(csvRawData, out isHeaderPresent, BomInterestHeaders, out RevisionZone);
            selectedRevisionVersion = CheckForRevisionHeader(csvRawData, out isRevisionPresent, RevisionZone);
            if (isRevisionPresent)
                selectedRevisionVersion = GetRevisionString(selectedRevisionVersion, out isRevisionPresent);
            if (isHeaderPresent && isRevisionPresent)
            {
                return filteredCSVData.Select(line =>
                {
                    string[] data = line.Split(COLON);
                    ReturnColons(data);
                    if (flag)
                        GetHeaderOffset(data, offset, out flag, BomInterestHeaders);
                    try
                    {
                        if (data?.Count() > offset[1] && data?.Count() > offset[0])
                        {
                            return (new BomInterestData(data[offset[0]], data[offset[1]]));
                        }
                        else
                        {
                            return (new BomInterestData(" ", " "));
                        }
                    }
                    catch(Exception e)
                    {
                        return (new BomInterestData(" ", " "));
                    }

                }).ToList();
            }
            MessageBox.Show(COMPONENET_ID_REFERENCE_NOT_PRESENT, ERROR);
            _memBomCSVList = null;
            selectedRevisionVersion = string.Empty;
            return (null);
        }

        private void SplitReferences(ICollection<BomInterestData> BOMList)
        {
            string LeftNumber = string.Empty;
            string RightNumber = string.Empty;
            string letters = string.Empty;
            string numbers = string.Empty;
            int startIndex;
            int endIndex;

            int listCount = BOMList.Count;
            string[] data;

            int scoreIndex;

            for (int i = 0; i < listCount; i++)
            {
                data = Regex.Split(BOMList.ElementAt(i).referenceDesignator, ",");

                if (data.Count() > 1 || data[0].Contains("-")) //was there at least one ","
                {
                    for (int x = 0; x < data.Count(); x++)
                    {
                        if (data[x].Contains("-")) //It is a range string ?
                        {
                            scoreIndex = data[x].IndexOf("-");
                            for (int y = 0; y < scoreIndex; y++)
                            {
                                if (char.IsNumber(data[x][y]))
                                    LeftNumber += data[x][y];
                                else
                                {
                                    if (x == 0)
                                        letters += data[x][y];
                                }

                            }
                            for (int z = (scoreIndex + 1); z < data[x].Length; z++)
                            {
                                if (char.IsNumber(data[x][z]))
                                    RightNumber += data[x][z];
                            }
                            Int32.TryParse(LeftNumber, out startIndex);
                            Int32.TryParse(RightNumber, out endIndex);
                            for (int w = startIndex; w < endIndex + 1; w++)
                            {
                                BOMList.Add(new BomInterestData(BOMList.ElementAt(i).componentId, string.Format("{0}{1}", letters, w.ToString())));
                            }
                            LeftNumber = string.Empty;
                            RightNumber = string.Empty;
                            startIndex = 0;
                            endIndex = 0;
                        }
                        else
                        {
                            for (int y = 0; y < data[x].Length; y++)
                            {
                                if (char.IsNumber(data[x][y]))
                                    numbers += data[x][y];
                                else
                                {
                                    if (x == 0)
                                    {
                                        letters += data[x][y];
                                    }
                                }
                            }
                            BOMList.Add(new BomInterestData(BOMList.ElementAt(i).componentId, string.Format("{0}{1}", letters, numbers)));
                            numbers = string.Empty;
                        }
                    }
                    BOMList.Remove(BOMList.ElementAt(i));
                    i--;
                    listCount--;
                    letters = string.Empty;
                }
            }
        }

        private void SpecialRanges(ICollection<BomInterestData> BOMList)
        {
            for (int i =0; i< BOMList.Count; i++)
            {
                if (string.IsNullOrEmpty(Regex.Replace(BOMList.ElementAt(i).componentId, " ", string.Empty)) && !string.IsNullOrEmpty(Regex.Replace(BOMList.ElementAt(i).referenceDesignator, " ", string.Empty)) && i > 0)
                {
                    if (!string.IsNullOrEmpty(BOMList.ElementAt(i - 1).componentId))
                    {
                        BOMList.ElementAt(i).componentId = BOMList.ElementAt(i - 1).componentId;
                    }
                }
            }
        }

        private string CheckForRevisionHeader(string[] rawData, out bool isRevisionPresent, int revisionZone)
        {
            string revisionVer = string.Empty;
            isRevisionPresent = true;
            for (int i = 0; i < revisionZone; i++)
            {
                if (Regex.Replace(rawData[i], " ", "").ToUpper().Contains(REVISION_QUOTE))
                {
                    return (revisionVer = rawData.ElementAt(i));
                }   
            }
            isRevisionPresent = false;
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

        private string[] GetFilteredData(string[] rawData, out bool isHeaderPresent, string[] headers, out int revisionZone)
        {
            isHeaderPresent = true;
            for (int i = 0; i < rawData.Count(); i++)
            {
                if (Regex.Replace(rawData[i], " ", "").ToUpper().Contains(headers[0]) && Regex.Replace(rawData[i], " ", "").ToUpper().Contains(headers[1]))
                {
                    revisionZone = i;
                    return (ReplaceQuoteMarks(rawData.Skip(i).ToArray()));
                }
            }
            revisionZone = 0;
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

        private string GetRevisionString(string revLine, out bool isRevisionPresent)
        {
            string[] data = Regex.Split(revLine,",");
            for (int i = 0; i<data.Count(); i++)
            {
                if (Regex.Replace(data[i], " ", "").ToUpper().Equals(REVISION_QUOTE))
                {
                    try
                    {
                        isRevisionPresent = true;
                        if (data?.Count() > i )
                        {
                            if (!(string.IsNullOrEmpty(Regex.Replace(data[i + 1], " ", string.Empty))))
                                return (data[i + 1].ToUpper());
                            else
                            {
                                isRevisionPresent = false;
                                return (null);
                            }
                        }
                        else
                        {
                            isRevisionPresent = false;
                            return (null);
                        }   
                    }
                    catch (Exception e)
                    {
                        isRevisionPresent = false;
                        return (null);
                    }
                }
            }
            isRevisionPresent = false;
            return (null);
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

        private string RemoveDotExtension(string fileString)
        {
            for (int i = (fileString.Length-1); i >= 0; i--)
            {
                if (fileString.ElementAt(i).Equals('.'))
                {
                    fileString = fileString.Remove(i, fileString.Length - i);
                    return (fileString);
                }
            }
            return (string.Empty);
        }

        private void DeleteBlankRowsCSV(ICollection<BomInterestData> bomList)
        {
            int listCount = bomList.Count;
            for (int i=0; i< listCount; i++)
            {
                if (string.IsNullOrEmpty(bomList.ElementAt(i).componentId.Trim()) && string.IsNullOrEmpty(bomList.ElementAt(i).referenceDesignator.Trim())){
                    bomList.Remove(bomList.ElementAt(i));
                    i--;
                    listCount--;
                }
            }
        }

        private void DelteBlankRowsTXT(ICollection<AoiInterestData> aoiList)
        {
            int listCount = aoiList.Count;
            for (int i = 0; i < listCount; i++)
            {
                if (string.IsNullOrEmpty(aoiList.ElementAt(i).partNumber.Trim()) && string.IsNullOrEmpty(aoiList.ElementAt(i).referenceDesignator.Trim()))
                {
                    aoiList.Remove(aoiList.ElementAt(i));
                    i--;
                    listCount--;
                }
            }
        }


        private void DeleteHeaders(ICollection<BomInterestData> bomList)
        {
            int listCount = bomList.Count;
            for (int i =1; i< (listCount); i++)
            {
                if (bomList.ElementAt(i).componentId.Equals(bomList.ElementAt(0).componentId) && bomList.ElementAt(i).referenceDesignator.Equals(bomList.ElementAt(0).referenceDesignator)){
                    bomList.Remove(bomList.ElementAt(i));
                    i--;
                    listCount--;
                }
            }
        }

        private void DeleteHeaders( ICollection<AoiInterestData> aoiList)
        {
            int listCount = aoiList.Count;
            for (int i = 1; i < (listCount); i++)
            {
                if (aoiList.ElementAt(i).partNumber.Equals(aoiList.ElementAt(0).partNumber) && aoiList.ElementAt(i).referenceDesignator.Equals(aoiList.ElementAt(0).referenceDesignator)){
                    aoiList.Remove(aoiList.ElementAt(i));
                    i--;
                    listCount--;
                }
            }
        }

        private void DeleteSpaces(ICollection<AoiInterestData> list)
        {
            for (int i =0; i<list.Count; i++)
            {
                list.ElementAt(i).partNumber = Regex.Replace(list.ElementAt(i).partNumber, " ", string.Empty);
                list.ElementAt(i).referenceDesignator = Regex.Replace(list.ElementAt(i).referenceDesignator, " ", string.Empty);
            }
        }

        private void DeleteSpaces(ICollection<BomInterestData> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.ElementAt(i).componentId = Regex.Replace(list.ElementAt(i).componentId, " ", string.Empty);
                list.ElementAt(i).referenceDesignator = Regex.Replace(list.ElementAt(i).referenceDesignator, " ", string.Empty);
            }
        }

        private bool IsRevisionVersionValid(string revisionVersion, string LastVersionInDB)
        {
            string newVersion = Regex.Replace(revisionVersion," ", string.Empty);
            string oldVersion = Regex.Replace(LastVersionInDB, " ", string.Empty);
            if (newVersion.Length > oldVersion.Length)
            {
                return (true);
            }
            else if (newVersion.Length == oldVersion.Length)
            {
                for (int i=0; i< newVersion.Length; i++)
                {
                    if (newVersion.ElementAt(i) > oldVersion.ElementAt(i))
                        return (true);
                    else if (newVersion.ElementAt(i) < oldVersion.ElementAt(i))
                        return (false);
                }
                return (false);
            }
            else
                return (false);

        }

        #endregion GeneralUse

    }
}
