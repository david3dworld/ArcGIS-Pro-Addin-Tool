using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using System.Web;
using System.Net;
using System.Reflection;

using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Controls;
using ArcGIS.Desktop.Mapping.Events;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Events;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.Controls;
using ArcGIS.Desktop.Editing.Events;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Core.Geoprocessing;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Ag_Analytics_Toolbar.ProfitLayer
{
    internal class ProfitLayerDockpaneViewModel : DockPane, IDataErrorInfo
    {
        private const string _dockPaneID = "Ag_Analytics_Toolbar_ProfitLayer_ProfitLayerDockpane";
        
        private string validationInputError = null;
        private string _validationSubmitError = null;

        private string _operationRasterPath = null;
        private readonly ICommand _operationRasterPathCommand;
        public ICommand OperationRasterPathCommand => _operationRasterPathCommand;

        private double _operationRasterCost = 1;

        private readonly ICommand _addOperationRasterCommand;
        public ICommand AddOperationRasterCommand => _addOperationRasterCommand;

        private readonly ICommand _removeOperationRasterCommand;
        public ICommand RemoveOperationRasterCommand => _removeOperationRasterCommand;

        private ObservableCollection<OperationRaster> _operationRasters = new ObservableCollection<OperationRaster>();
        private OperationRaster _selectedOperationRaster = null;

        private string _varietyLayerPath = null;
        private readonly ICommand _varietyLayerPathCommand;
        public ICommand VarietyLayerPathCommand => _varietyLayerPathCommand;

        private string _varietyDBFfilePath = null;
        private readonly ICommand _varietyDBFfilePathCommand;
        public ICommand VarietyDBFfilePathCommand => _varietyDBFfilePathCommand;

        private int _constantAdd = 1;
        private double _cellSize = 0.0001;

        private string _downloadPath = null;
        private readonly ICommand _downloadPathCommand;
        public ICommand DownloadPathCommand => _downloadPathCommand;

        private readonly ICommand _submitCommand;
        public ICommand SubmitCommand => _submitCommand;
        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;

        private string _progressVisible = "Hidden";

        protected ProfitLayerDockpaneViewModel()
        {
            _operationRasterPathCommand = new RelayCommand(() => OperationRasterPathExecute(), () => true);
            
            _addOperationRasterCommand = new RelayCommand(() => AddOperationRasterExecute(), () => true);
            _removeOperationRasterCommand = new RelayCommand(() => RemoveOperationRasterExecute(), () => true);

            _varietyLayerPathCommand = new RelayCommand(() => VarietyLayerPathExecute(), () => true);
            _varietyDBFfilePathCommand = new RelayCommand(() => VarietyDBFfilePathExecute(), () => true);
            _downloadPathCommand = new RelayCommand(() => DownloadPathExecute(), () => true);

            _submitCommand = new RelayCommand(() => SubmitExecute(), () => true);
            _cancelCommand = new RelayCommand(() => CancelExecute(), () => true);
        }

        protected override Task InitializeAsync()
        {
            ProjectItemsChangedEvent.Subscribe(OnProjectCollectionChanged);
            return base.InitializeAsync();
        }

        private void OnProjectCollectionChanged(ProjectItemsChangedEventArgs args)
        {
            DownloadPath = Project.Current.DefaultGeodatabasePath;
        }

        /// <summary>
        /// Show the DockPane.
        /// </summary>
        internal static void Show()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Activate();
        }

        protected override void OnHelpRequested()
        {
            //System.Diagnostics.Process.Start(@"");
        }

        public void CancelExecute()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Hide();
        }

        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "ProfitLayer Parameters";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }
        
        public string OperationRasterPath
        {
            get { return _operationRasterPath; }
            set
            {
                SetProperty(ref _operationRasterPath, value, () => OperationRasterPath);
            }
        }
        
        public double OperationRasterCost
        {
            get { return _operationRasterCost; }
            set
            {
                SetProperty(ref _operationRasterCost, value, () => OperationRasterCost);
            }
        }

        public ObservableCollection<OperationRaster> OperationRasters
        {
            get { return _operationRasters; }
            set
            {
                SetProperty(ref _operationRasters, value, () => OperationRasters);
            }
        }

        public OperationRaster SelectedOperationRaster
        {
            get { return _selectedOperationRaster; }
            set
            {
                SetProperty(ref _selectedOperationRaster, value, () => SelectedOperationRaster);
                if(value != null)
                {
                    OperationRasterPath = value.Raster_Path;
                    OperationRasterCost = value.Raster_Cost;
                }
            }
        }

        public string VarietyLayerPath
        {
            get { return _varietyLayerPath; }
            set
            {
                SetProperty(ref _varietyLayerPath, value, () => VarietyLayerPath);
            }
        }

        public string VarietyDBFfilePath
        {
            get { return _varietyDBFfilePath; }
            set
            {
                SetProperty(ref _varietyDBFfilePath, value, () => VarietyDBFfilePath);
            }
        }

        public int ConstantAdd
        {
            get { return _constantAdd; }
            set
            {
                SetProperty(ref _constantAdd, value, () => ConstantAdd);
            }
        }

        public double CellSize
        {
            get { return _cellSize; }
            set
            {
                SetProperty(ref _cellSize, value, () => CellSize);
            }
        }

        public string DownloadPath
        {
            get { return _downloadPath; }
            set
            {
                SetProperty(ref _downloadPath, value, () => DownloadPath);
            }
        }
        
        public string ProgressVisible
        {
            get { return _progressVisible; }
            set
            {
                SetProperty(ref _progressVisible, value, () => ProgressVisible);
            }
        }

        public string ValidationSubmitError
        {
            get { return _validationSubmitError; }
            set
            {
                SetProperty(ref _validationSubmitError, value, () => ValidationSubmitError);
            }
        }

        public string Error
        {
            get

            {
                return this[string.Empty];
            }
        }

        public string this[string cuurectname]
        {
            get
            {
                validationInputError = null;

                switch (cuurectname)
                {
                    case "CellSize":
                        if (CellSize < 0.000025 || CellSize > 1)
                        {
                            validationInputError = "This value must be between 0.000025 and 1.";
                        }
                        break;
                    default:
                        break;
                }
                return validationInputError;
            }
        }

        public void OperationRasterPathExecute()
        {
            //Display the filter in an Open Item dialog
            BrowseProjectFilter bf = new BrowseProjectFilter("esri_browseDialogFilters_browseFiles");
            bf.Name = "TIF Files";
            bf.FileExtension = "*.tif";
            bf.BrowsingFilesMode = true;

            OpenItemDialog dlg = new OpenItemDialog
            {
                Title = "Open TIF File",
                InitialLocation = _operationRasterPath,
                AlwaysUseInitialLocation = true,
                MultiSelect = false,
                BrowseFilter = bf
            };

            bool? ok = dlg.ShowDialog();

            if (ok == true)
            {
                var item = dlg.Items.First();
                OperationRasterPath = item.Path;
            }
        }

        public void AddOperationRasterExecute()
        {
            if(_operationRasterPath == null || string.IsNullOrEmpty(_operationRasterPath))
            {
                return;
            }
            else
            {
                if (!File.Exists(_operationRasterPath))
                {
                    return;
                }
            }

            OperationRaster newOperationRaster = new OperationRaster();
            newOperationRaster.Raster_Name = Path.GetFileName(_operationRasterPath);
            newOperationRaster.Raster_Path = _operationRasterPath;
            newOperationRaster.Raster_Cost = _operationRasterCost;

            OperationRasters.Add(newOperationRaster);
        }
        
        public void RemoveOperationRasterExecute()
        {
            if(_selectedOperationRaster != null)
            {
                OperationRasters.Remove(_selectedOperationRaster);
            }
        }

        public void VarietyLayerPathExecute()
        {
            //Display the filter in an Open Item dialog
            BrowseProjectFilter bf = new BrowseProjectFilter("esri_browseDialogFilters_browseFiles");
            bf.Name = "TIF Files";
            bf.FileExtension = "*.tif";
            bf.BrowsingFilesMode = true;

            OpenItemDialog dlg = new OpenItemDialog
            {
                Title = "Open TIF File",
                InitialLocation = _varietyLayerPath,
                AlwaysUseInitialLocation = true,
                MultiSelect = false,
                BrowseFilter = bf
            };

            bool? ok = dlg.ShowDialog();

            if (ok == true)
            {
               var item = dlg.Items.First();
               VarietyLayerPath = item.Path;
            }

        }

        public void VarietyDBFfilePathExecute()
        {
            //Display the filter in an Open Item dialog
            BrowseProjectFilter bf = new BrowseProjectFilter("esri_browseDialogFilters_browseFiles");
            bf.Name = "DBF Files";
            bf.FileExtension = "*.dbf";
            bf.BrowsingFilesMode = true;

            OpenItemDialog dlg = new OpenItemDialog
            {
                Title = "Open TIF File",
                InitialLocation = _varietyDBFfilePath,
                AlwaysUseInitialLocation = true,
                MultiSelect = false,
                BrowseFilter = bf
            };

            bool? ok = dlg.ShowDialog();

            if (ok == true)
            {
                var item = dlg.Items.First();
                VarietyDBFfilePath = item.Path;
            }
        }

        public void DownloadPathExecute()
        {
            //Display the filter in an Open Item dialog
            BrowseProjectFilter bf = new BrowseProjectFilter();
            bf.Name = "Folders and Geodatabases";

            bf.AddFilter(BrowseProjectFilter.GetFilter(ItemFilters.geodatabases));
            bf.AddFilter(BrowseProjectFilter.GetFilter(ItemFilters.folders));

            bf.Includes.Add("FolderConnection");
            bf.Includes.Add("GDB");
            bf.Excludes.Add("esri_browsePlaces_Online");

            OpenItemDialog dlg = new OpenItemDialog
            {
                Title = "Open Folder Dialog",
                InitialLocation = _downloadPath,
                AlwaysUseInitialLocation = true,
                MultiSelect = false,
                BrowseFilter = bf
            };

            bool? ok = dlg.ShowDialog();

            if (ok == true)
            {
                var item = dlg.Items.First();

                DownloadPath = item.Path;
            }
        }

        public async void SubmitExecute()
        {
            ValidationSubmitError = null;
            List<string> validationSubmitErrors = new List<string>();
            
            if(_operationRasters.Count == 0)
            {
                validationSubmitErrors.Add("Operation Rasters must be added.");
            }
            if(_varietyLayerPath != null && !string.IsNullOrEmpty(_varietyLayerPath))
            {
                if (!File.Exists(_varietyLayerPath))
                {
                    validationSubmitErrors.Add("Variety Layer Path doesn't exsist.");
                }
            }
            if(_varietyDBFfilePath !=null && !string.IsNullOrEmpty(_varietyDBFfilePath))
            {
                if (!File.Exists(_varietyDBFfilePath))
                {
                    validationSubmitErrors.Add("Variety DBF file Path doesn't exsist.");
                }
            }
            if (_downloadPath == null || string.IsNullOrEmpty(_downloadPath))
            {
                validationSubmitErrors.Add("Download path must be selected.");
            }
            else
            {
                if (!Directory.Exists(_downloadPath))
                {
                    validationSubmitErrors.Add("Download path doesn't exsist.");
                }
            }
            if (validationSubmitErrors.Count > 0)
            {
                ValidationSubmitError = string.Join("\n", validationSubmitErrors);
                return;
            }

            if (validationInputError != null)
            {
                return;
            }

            ProgressVisible = "Visible";

            await QueuedTask.Run(async () =>
            {
                var client = new RestClient("https://analytics.ag/api/ToolBoxProxy/ProfitLayer");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;

                List<string> raster_cost_array = new List<string>();
                foreach (OperationRaster raster in _operationRasters)
                {
                    request.AddFile("rasters", raster.Raster_Path);
                    raster_cost_array.Add(raster.Raster_Cost.ToString());
                }
                string constants_vector = string.Join(",", raster_cost_array);

                if (_varietyLayerPath != null && !string.IsNullOrEmpty(_varietyLayerPath))
                {
                    request.AddFile("category_raster", _varietyLayerPath);
                }
                if (_varietyDBFfilePath != null && !string.IsNullOrEmpty(_varietyDBFfilePath))
                {
                    
                    request.AddFile("dbf_file", _varietyDBFfilePath);
                }
                request.AddParameter("constants_vector", constants_vector);
                request.AddParameter("constant_add", _constantAdd);
                request.AddParameter("cell_size", _cellSize);
                request.AddParameter("Token", "v4289wyrwIShfgIWQO4DFWawrzf");

                IRestResponse response = client.Execute(request);
                
                if (!response.IsSuccessful)
                {
                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(response.ErrorMessage);
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed Result. Please try again");
                    return;
                }
                
                string content = response.Content;
                string unescapedJsonString = JsonConvert.DeserializeObject<dynamic>(content);

                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(unescapedJsonString);

                try
                {
                   string filename = jsonData.file;
                   await ExportFile(_downloadPath, filename);
                }
                catch (Exception e)
                {
                    if (jsonData.GetType().GetProperty("msg") == null)
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("No Result");
                    }
                    else
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(jsonData.msg);
                    }
                }

            });

            ProgressVisible = "Hidden";
        }

        private async Task DownloadFile(string download_path, string filename)
        {
            await QueuedTask.Run(() => {

                var download_client = new RestClient("https://analytics.ag/api/ToolBoxProxy/ProfitLayer?filename=" + filename);
                download_client.Timeout = -1;
                var download_request = new RestRequest(Method.GET);
                download_request.AlwaysMultipartFormData = true;

                //download_client.DownloadData(request).SaveAs("C:/result.tif");
                byte[] download_response = download_client.DownloadData(download_request);
                File.WriteAllBytes(Path.Combine(download_path, "ProfitLayer_" + filename), download_response);
            });
        }

        private async Task ExportFile(string download_path, string filename)
        {
            await QueuedTask.Run(async () => {

                try
                {
                    Geodatabase gdb = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(@download_path)));

                    string default_path = Path.GetDirectoryName(Project.Current.URI);
                    
                    await DownloadFile(default_path, filename);

                    string fullPath = Path.Combine(default_path, "ProfitLayer_" + filename);

                    string rasterFileName = Path.GetFileNameWithoutExtension(fullPath);

                    string rasterName = Regex.Replace(rasterFileName, @"[^0-9a-zA-Z_]", "_");  //string.Empty

                    string outputRaster = Path.Combine(download_path, rasterName);

                    await Ag_Analytics_Module.CopyRaster(fullPath, outputRaster);

                    await Ag_Analytics_Module.SetToClassifyColorizerFromLayerName(rasterName, 10, "Bathymetric Scale");

                    // delete files in default path
                    File.Delete(fullPath);
                }
                catch
                {
                    await DownloadFile(download_path, filename);

                    await Ag_Analytics_Module.AddLayerToMapAsync(Path.Combine(download_path, "ProfitLayer_" + filename));

                    await Ag_Analytics_Module.SetToClassifyColorizerFromLayerName("ProfitLayer_" + filename, 10, "Bathymetric Scale");
                }
            });
        }
    }

    public class OperationRaster
    {
        public string Raster_Name
        {
            get;
            set;
        }
        public string Raster_Path
        {
            get;
            set;
        }
        public double Raster_Cost
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class ProfitLayerDockpane_ShowButton : Button
    {
        protected override void OnClick()
        {
            ProfitLayerDockpaneViewModel.Show();
        }
    }
}
