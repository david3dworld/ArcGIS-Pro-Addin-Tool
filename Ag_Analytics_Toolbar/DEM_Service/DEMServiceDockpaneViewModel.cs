using System;
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

using System.IO;
using System.Web;
using System.Net;
using System.Reflection;

using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Events;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Controls;
using ArcGIS.Desktop.Mapping.Events;
using ArcGIS.Desktop.Core.Geoprocessing;


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

using Ag_Analytics_Toolbar.CoordinateSystemDialog;

namespace Ag_Analytics_Toolbar.DEM_Service
{
    internal class DEMServiceDockpaneViewModel : DockPane, IDataErrorInfo
    {
        private const string _dockPaneID = "Ag_Analytics_Toolbar_DEM_Service_DEMServiceDockpane";

        private string validationInputError = null;
        private string _validationSubmitError = null;

        private ObservableCollection<FeatureLayer> _AOILayers = new ObservableCollection<FeatureLayer>();
        private FeatureLayer _selectedAOILayer = null;

        private double _cellSize = 0.0001;
        
        private bool _checkElevationIndex = false;
        
        private SpatialReference selectedSpatialReference = null;
        private string _coordinateSystem = null;
        
        private string _downloadPath = null;

        private readonly ICommand _zoomToLayerCommand;
        public ICommand ZoomToLayerCommand => _zoomToLayerCommand;

        private CoordSysDialog _dlg = null;
        private static bool _isOpen = false;
        private readonly ICommand _openCoordinateSystemCommand;
        public ICommand OpenCoordinateSystemCommand => _openCoordinateSystemCommand;
        
        private readonly ICommand _downloadPathCommand;
        public ICommand DownloadPathCommand => _downloadPathCommand;

        private readonly ICommand _submitCommand;
        public ICommand SubmitCommand => _submitCommand;
        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;

        protected DEMServiceDockpaneViewModel()
        {
            _zoomToLayerCommand = new RelayCommand(() => ZoomToLayerExecute(), () => true);
            _openCoordinateSystemCommand = new RelayCommand(() => OpenCoordinateSystemExecute(), () => true);
            _downloadPathCommand = new RelayCommand(() => DownloadPathExecute(), () => true);
            _submitCommand = new RelayCommand(() => SubmitExecute(), () => true);
            _cancelCommand = new RelayCommand(() => CancelExecute(), () => true);
            
            SetAOILayers();
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
            System.Diagnostics.Process.Start(@"https://ag-analytics.portal.azure-api.net/docs/services/dem-service/operations/dem-service");
        }
        
        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "DEM Service Parameters";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        protected override Task InitializeAsync()
        {
            ProjectItemsChangedEvent.Subscribe(OnProjectCollectionChanged);

            ActiveMapViewChangedEvent.Subscribe(OnActiveMapViewChangedEvent);

            LayersAddedEvent.Subscribe(OnLayersAddedEvent);
            LayersRemovedEvent.Subscribe(OnLayersRemovedEvent);

            return base.InitializeAsync();
        }

        private void OnProjectCollectionChanged(ProjectItemsChangedEventArgs args)
        {
            SetAOILayers();
            //DownloadPath = Path.GetDirectoryName(Project.Current.URI);
            DownloadPath = Project.Current.DefaultGeodatabasePath;
        }

        private void OnActiveMapViewChangedEvent(ActiveMapViewChangedEventArgs obj)
        {

            if (obj.IncomingView == null)
            {
                // there is no active map view - disable the UI
                AOILayers = null;
                SelectedAOILayer = null;
                return;
            }
            // we have an active map view - enable the UI
            SetAOILayers();
        }

        private void OnLayersAddedEvent(LayerEventsArgs args)
        {
            SetAOILayers();
        }

        private void OnLayersRemovedEvent(LayerEventsArgs args)
        {
            SetAOILayers();
        }

        public void CancelExecute()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Hide();
        }

        public ObservableCollection<FeatureLayer> AOILayers
        {
            get { return _AOILayers; }
            set
            {
                SetProperty(ref _AOILayers, value, () => AOILayers);
            }

        }

        public FeatureLayer SelectedAOILayer
        {
            get { return _selectedAOILayer; }

            set
            {
                SetProperty(ref _selectedAOILayer, value, () => SelectedAOILayer);
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

        public bool CheckElevationIndex
        {
            get { return _checkElevationIndex; }
            set
            {
                SetProperty(ref _checkElevationIndex, value, () => CheckElevationIndex);
            }
        }

        public string CoordinateSystem
        {
            get { return _coordinateSystem; }
            set
            {
                SetProperty(ref _coordinateSystem, value, () => CoordinateSystem);
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

        public async void ZoomToLayerExecute()
        {
            if (MapView.Active != null)
            {
                if (_selectedAOILayer != null)
                {
                    await MapView.Active.ZoomToAsync(_selectedAOILayer);
                }
            }
        }

        public void OpenCoordinateSystemExecute()
        {
            if (_isOpen)
                return;
            _isOpen = true;
            _dlg = new CoordSysDialog();
            _dlg.Closing += bld_Closing;
            _dlg.Owner = FrameworkApplication.Current.MainWindow;
            _dlg.Show();
        }

        private void bld_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_dlg.SpatialReference != null)
            {
                //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(string.Format("You picked {0}", _dlg.SpatialReference.Name), "Pick Coordinate System");

                CoordinateSystem = _dlg.SpatialReference.Name;
                selectedSpatialReference = _dlg.SpatialReference;
            }
            _dlg = null;
            _isOpen = false;
        }

        public void DownloadPathExecute()
        {
            //Display the filter in an Open Item dialog
            BrowseProjectFilter bf = new BrowseProjectFilter();
            bf.Name = "Folders and Geodatabases ";

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

        private async void SetAOILayers()
        {
            try
            {
                // Gets the first 2D map from the project that is called Map.
                //Map _map = await GetMapFromProject(Project.Current, "Map");
                if (MapView.Active == null)
                {
                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("SetAOILayers: map is not active.");
                    return;
                }

                var _map = MapView.Active.Map;

                List<FeatureLayer> featureLayerList = _map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();

                ObservableCollection<FeatureLayer> newAOILayers = new ObservableCollection<FeatureLayer>();

                foreach (FeatureLayer featureLayer in featureLayerList)
                {
                    if (featureLayer.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        int featureCount = await QueuedTask.Run(() => { return featureLayer.GetFeatureClass().GetCount(); });
                        if (featureCount < 2)
                        {
                            newAOILayers.Add(featureLayer);
                        }
                    }
                }
                AOILayers = newAOILayers;
                if (AOILayers.Count > 0)
                {
                    SelectedAOILayer = AOILayers.First();
                }
                else
                {
                    SelectedAOILayer = null;
                }
            }
            catch (Exception exc)
            {
                // Catch any exception found and display a message box.
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception caught while trying to get layers: " + exc.Message);
                return;
            }
        }

        public async void SubmitExecute()
        {
            ValidationSubmitError = null;

            List<string> validationSubmitErrors = new List<string>();

            string aoi = null;
            if (_selectedAOILayer != null)
            {
                int featureCount = await QueuedTask.Run(() => { return _selectedAOILayer.GetFeatureClass().GetCount(); });
                if (featureCount == 0)
                {
                    validationSubmitErrors.Add("AOI is empty.");
                }
                else if (featureCount == 1)
                {
                    aoi = await Ag_Analytics_Module.GetGeoJSONFromFeatureLayer(_selectedAOILayer);
                }
                else if (featureCount > 1)
                {
                    validationSubmitErrors.Add("AOI must be contained only a polygon feature");
                }
            }
            else
            {
                validationSubmitErrors.Add("AOI parameter must be selected.");
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

            SpatialReference outputSpatialReference = null;
            if (selectedSpatialReference == null)
            {
                outputSpatialReference = await QueuedTask.Run(() => { return _selectedAOILayer.GetSpatialReference(); });
            }
            else
            {
                outputSpatialReference = selectedSpatialReference;
            }
            
            if(outputSpatialReference.IsGeographic && _cellSize > 1)
            {
                validationSubmitErrors.Add("Resolution must be < 1 in geographic coordinate system(ex:0.0001)");
            }
            else if(outputSpatialReference.IsProjected && _cellSize < 1)
            {
                validationSubmitErrors.Add("Resolution must be > 1 in projected coordinate system(ex:10)");
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

            ProgressDialog progressDialog;
            progressDialog = new ProgressDialog("Please wait for result response...");
            progressDialog.Show();

            await QueuedTask.Run(async () =>
            {
                var client = new RestClient("https://ag-analytics.azure-api.net/dem-service/");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);

                //request.AlwaysMultipartFormData = true;
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                
                request.AddParameter("aoi", aoi);

                request.AddParameter("Resolution", _cellSize);
                request.AddParameter("Elevation_Index", _checkElevationIndex);

                request.AddParameter("Projection", outputSpatialReference.Wkt);

                // these parameter options no need on ArcGIS pro
                request.AddParameter("Legend_Ranges", 1);
               
                IRestResponse response = client.Execute(request);

                if (!response.IsSuccessful)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed Result. Please try again.");
                    return;
                }

                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content);

                try
                {
                        string filename = jsonData.FileName;

                        await ExportFile(_downloadPath, filename);
                }
                catch (Exception e)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("No Result");
                }

            });

            progressDialog.Hide();

        }

        private async Task DownloadFile(string download_path, string filename)
        {
            await QueuedTask.Run(() =>
            {
                var download_client = new RestClient("https://ag-analytics.azure-api.net/dem-service/?FileName=" + filename);
                download_client.Timeout = -1;
                var download_request = new RestRequest(Method.GET);
                download_request.AlwaysMultipartFormData = true;

                //download_client.DownloadData(request).SaveAs("C:/result.tif");
                byte[] download_response = download_client.DownloadData(download_request);
                File.WriteAllBytes(Path.Combine(download_path, filename), download_response);
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

                    string fullPath = Path.Combine(default_path, filename);

                    string rasterFileName = Path.GetFileNameWithoutExtension(fullPath);

                    string rasterName = Regex.Replace(rasterFileName, @"[^0-9a-zA-Z_]", "_");  //string.Empty

                    string outputRaster = Path.Combine(download_path, rasterName);

                    await Ag_Analytics_Module.CopyRaster(fullPath, outputRaster);

                    await Ag_Analytics_Module.SetToStretchColorizerFromLayerName(rasterFileName, "Elevation #10");

                    // delete files in default path
                    File.Delete(fullPath);
                }
                catch
                {
                    await DownloadFile(download_path, filename);

                    await Ag_Analytics_Module.AddLayerToMapAsync(Path.Combine(download_path, filename));

                    await Ag_Analytics_Module.SetToStretchColorizerFromLayerName(filename, "Elevation #10");
                }
            });
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

                        if (CellSize <= 0)
                        {
                            validationInputError = "Resolution must be > 0.";
                        }
                        break;
                    default:
                        break;
                }
                return validationInputError;
            }
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class DEMServiceDockpane_ShowButton : Button
    {
        protected override void OnClick()
        {
            DEMServiceDockpaneViewModel.Show();
        }
    }
}
