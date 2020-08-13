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

using Ag_Analytics_Toolbar.CoordinateSystemDialog;

namespace Ag_Analytics_Toolbar.HLS_Service
{
    internal class HLSDockpaneViewModel : DockPane
    {
        private const string _dockPaneID = "Ag_Analytics_Toolbar_HLS_Service_HLSDockpane";

        //private readonly object _lockCollection = new object();

        private ObservableCollection<FeatureLayer> _polygonFeatureLayers = new ObservableCollection<FeatureLayer>();
        private FeatureLayer _selectedPolygonFeatureLayer = null;
        
        private ObservableCollection<HLS_Band> _bands = new ObservableCollection<HLS_Band>();

        private ObservableCollection<string> _satellites = new ObservableCollection<string>();
        private string _selectedSatellite = "";

        private bool _showDate = false;
        
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now;

        private float _cellSize = 20;
        private float _qacloudperc = 100;
        private float _displaynormalvalues = 2000;

        private bool _checkbyweek = true;
        private bool _checkfilter = false;
        private bool _checkqafilter = false;
        private bool _checkflattendata = false;       

        private SpatialReference selectedSpatialReference =  null;
        private string _coordinateSystem = "";
        private string _downloadPath = "";

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
        
        public HLSDockpaneViewModel() {
            
            string[] band_names = { 
                "Red","Green","Blue","NIR","NIR_Broad","Red_Edge_1","Red_Edge_2","Red_edge_3","SWIR1","SWIR2","Coastal Aerosol","QA",
                "NDVI", "RGB","NDWI","NDBI","NDTI","UI","GCVI","MTCI","NDRE",
                "CIR","UE","LW","AP","AGR","FFBS","BE","VW"
            };
            foreach (string band_name in band_names)
            {
                HLS_Band obj = new HLS_Band();
                obj.Band_Name = band_name;
                _bands.Add(obj);
            }

            _satellites.Add("Landsat");
            _satellites.Add("Sentinel");
            _satellites.Add("Landsat, Sentinel");
            _selectedSatellite = _satellites.First();

            _zoomToLayerCommand = new RelayCommand(() => ZoomToLayerExecute(), () => true);
            _openCoordinateSystemCommand = new RelayCommand(() => OpenCoordinateSystemExecute(), () => true);
            _downloadPathCommand = new RelayCommand(() => DownloadPathExecute(), () => true);
            _submitCommand = new RelayCommand(() => SubmitExecute(), () => true);
            _cancelCommand = new RelayCommand(() => CancelExecute(), () => true);

            SetPolygonFeatureLayers();
            
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
            SetPolygonFeatureLayers();
            //DownloadPath = Path.GetDirectoryName(Project.Current.URI);
            DownloadPath = Project.Current.DefaultGeodatabasePath;
        }
        
        private void OnActiveMapViewChangedEvent(ActiveMapViewChangedEventArgs obj)
        {
            
            if (obj.IncomingView == null)
            {
                // there is no active map view - disable the UI
                PolygonFeatureLayers = null;
                SelectedPolygonFeatureLayer = null;
                return;
            }
            // we have an active map view - enable the UI
            SetPolygonFeatureLayers();
           
        }
        
        private void OnLayersAddedEvent(LayerEventsArgs args)
        {
            /*
            foreach (var newLayer in args.Layers)
            {
                if (newLayer.Map == MapView.Active.Map && newLayer is FeatureLayer)
                {
                    PolygonFeatureLayers.Add((FeatureLayer)newLayer);
                }
            }
            */
           
            SetPolygonFeatureLayers();
        }

        private void OnLayersRemovedEvent(LayerEventsArgs args)
        {
           
            SetPolygonFeatureLayers();
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
        
        /// <summary>
        /// Text shown near the top of the DockPane.
        /// </summary>
        private string _heading = "HLS Request Parameters Setting";
        public string Heading
        {
            get { return _heading; }
            set
            {
                SetProperty(ref _heading, value, () => Heading);
            }
        }

        protected override void OnHelpRequested()
        {
            System.Diagnostics.Process.Start(@"https://ag-analytics.portal.azure-api.net/docs/services/harmonized-landsat-sentinel-service/operations/hls-service");
        }

        public void CancelExecute()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Hide();
        }

        public ObservableCollection<FeatureLayer> PolygonFeatureLayers
        {
            get { return _polygonFeatureLayers; }
            set
            {
                SetProperty(ref _polygonFeatureLayers, value, () => PolygonFeatureLayers);
            }
            
        }

        public FeatureLayer SelectedPolygonFeatureLayer
        {
            get {  return _selectedPolygonFeatureLayer;   }
            
            set
            {
                SetProperty(ref _selectedPolygonFeatureLayer, value, () => SelectedPolygonFeatureLayer);
            }
        }

        public ObservableCollection<HLS_Band> Bands
        {
            get { return _bands; }
            set
            {
                SetProperty(ref _bands, value, () => Bands);
            }
        }
        
        public ObservableCollection<string> Satellites
        {
            get { return _satellites; }
            set
            {
                SetProperty(ref _satellites, value, () => Satellites);
            }
        }

        public string SelectedSatellite
        {
            get { return _selectedSatellite; }
            set
            {
                SetProperty(ref _selectedSatellite, value, () => SelectedSatellite);
            }
        }
        
        public bool ShowDate
        {
            get { return _showDate; }
            set
            {
                SetProperty(ref _showDate, value, () => ShowDate);
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                SetProperty(ref _startDate, value, () => StartDate);
            }
        }
        
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                SetProperty(ref _endDate, value, () => EndDate);
            }
        }

        public bool CheckByWeek
        {
            get { return _checkbyweek; }
            set
            {
                SetProperty(ref _checkbyweek, value, () => ShowDate);
            }
        }

        public bool CheckFilter
        {
            get { return _checkfilter; }
            set
            {
                SetProperty(ref _checkfilter, value, () => ShowDate);
            }
        }

        public bool CheckQaFilter
        {
            get { return _checkqafilter; }
            set
            {
                SetProperty(ref _checkqafilter, value, () => ShowDate);
            }
        }

        public bool CheckFlattenData
        {
            get { return _checkflattendata ; }
            set
            {
                SetProperty(ref _checkflattendata, value, () => ShowDate);
            }
        }

        public float CellSize
        {
            get { return _cellSize; }
            set
            {
                SetProperty(ref _cellSize, value, () => CellSize);
            }
        }

        public float QaCloudPerc
        {
            get { return _qacloudperc; }
            set
            {
                SetProperty(ref _qacloudperc, value, () => CellSize);
            }
        }

        public float DisplayNormalValues
        {
            get { return _displaynormalvalues; }
            set
            {
                SetProperty(ref _displaynormalvalues, value, () => CellSize);
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
            if(MapView.Active != null)
            {
                if(_selectedPolygonFeatureLayer != null)
                {
                    await MapView.Active.ZoomToAsync(_selectedPolygonFeatureLayer);
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
            
            if(ok == true)
            {
                var item = dlg.Items.First();
                
                DownloadPath = item.Path;
            }
        }

        public async void SubmitExecute()
        {

            string aoi = "";
            if (_selectedPolygonFeatureLayer != null)
            {
                aoi = await Ag_Analytics_Module.GetGeoJSONFromFeatureLayer(_selectedPolygonFeatureLayer);
            }
            else
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please select AOI Layer");
                return;
            }

            string selectedBands = "";
            List<string> _selectedBands = new List<string>();
            foreach(var band in _bands)
            {
                if(band.Check_Status == true)
                {
                    _selectedBands.Add(band.Band_Name);
                }
            }
            if(_selectedBands.Count > 0)
            {
                selectedBands = JsonConvert.SerializeObject(_selectedBands);
            }
            else
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please select bands");
                return;
            }
            
            //satellite: _selectedSatellite
            int _showLatest = _showDate ? 0 : 1;
            string startDate = String.Format("{0:M/d/yyyy}", _startDate);
            string endDate = String.Format("{0:M/d/yyyy}", _endDate);
            
            if(_qacloudperc < 0 || _qacloudperc > 100)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("qacloudperc input error: qacloudperc is between 0~100.");
                return;
            }

            string coordSysString = "";
            if (selectedSpatialReference == null)
            {
                coordSysString = await QueuedTask.Run(() => { return _selectedPolygonFeatureLayer.GetSpatialReference().Wkt; });
            }
            else
            {
                coordSysString = selectedSpatialReference.Wkt;
            }

            if (_downloadPath == "")
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please set download folder path.");
                return;
            }
            else
            {
                if (!Directory.Exists(_downloadPath))
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Download path Error!");
                    return;
                }
            }

            ArcGIS.Desktop.Framework.Threading.Tasks.ProgressDialog progressDialog;
            progressDialog = new ArcGIS.Desktop.Framework.Threading.Tasks.ProgressDialog("Please wait for result response...");
            progressDialog.Show();

            await QueuedTask.Run(async () =>
            {
                var client = new RestClient("https://ag-analytics.azure-api.net/hls-service/");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                
                //request.AlwaysMultipartFormData = true;
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                request.AddParameter("aoi", aoi);
                request.AddParameter("Band", selectedBands);
                request.AddParameter("satellite", _selectedSatellite);
               
                request.AddParameter("showlatest", _showLatest);
                request.AddParameter("Startdate", startDate);
                request.AddParameter("Enddate", endDate);

                request.AddParameter("resolution", _cellSize);
                request.AddParameter("displaynormalvalues", _displaynormalvalues);
                request.AddParameter("qacloudperc", _qacloudperc);

                request.AddParameter("byweek", _checkbyweek?1:0);
                request.AddParameter("filter", _checkfilter?1:0);
                request.AddParameter("qafilter", _checkqafilter?1:0);
                request.AddParameter("flatten_data", _checkflattendata?1:0);
                
                request.AddParameter("projection", coordSysString);

                // these parameter options no need on ArcGIS pro
                request.AddParameter("legendtype", "Relative"); 
                request.AddParameter("statistics", 0);  // set always 0
                request.AddParameter("return_tif", 1);  // set always 1
                
                IRestResponse response = client.Execute(request);
                
                if (!response.IsSuccessful)
                {
                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(response.ErrorMessage);
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed Result");
                    return;
                }

                // need to add API Error handling

                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(response.Content);

                try
                {
                    foreach (var item in jsonData)
                    {
                        string filename = item.download_url;
                        await ExportFile(_downloadPath, filename);
                    }
                }
                catch(Exception e)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("No Result");
                }
                
            });

            progressDialog.Hide();
            
        }

        private async Task DownloadFile(string download_path, string filename)
        {
            await QueuedTask.Run(() => {

                var download_client = new RestClient("https://ag-analytics.azure-api.net/hls-service/?filename=" + filename);
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

                    string fullPath = Path.Combine(default_path, filename);

                    await DownloadFile(default_path, filename);

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

                    await Ag_Analytics_Module.AddRasterLayerToMapAsync(Path.Combine(download_path, filename));

                    await Ag_Analytics_Module.SetToClassifyColorizerFromLayerName(filename, 10, "Bathymetric Scale");
                }
            });
        }

        private void SetPolygonFeatureLayers()
        {
            try
            {
                // Gets the first 2D map from the project that is called Map.
                //Map _map = await GetMapFromProject(Project.Current, "Map");
                if(MapView.Active == null)
                {
                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("SetPolygonFeatureLayers: map is not active.");
                    return;
                }
               
                var _map = MapView.Active.Map;
                
                List<FeatureLayer> featureLayerList = _map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();

                ObservableCollection<FeatureLayer> newPolygonFeatureLayers = new ObservableCollection<FeatureLayer>();
                
                foreach(FeatureLayer featureLayer in featureLayerList)
                {
                    if(featureLayer.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        newPolygonFeatureLayers.Add(featureLayer);
                    }
                }
                PolygonFeatureLayers = newPolygonFeatureLayers;
                if(PolygonFeatureLayers.Count > 0)
                {
                    SelectedPolygonFeatureLayer = PolygonFeatureLayers.First();
                }
                else
                {
                    SelectedPolygonFeatureLayer = null;
                }
            }
            catch (Exception exc)
            {
                // Catch any exception found and display a message box.
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception caught while trying to get layers: " + exc.Message);
                return;
            }
        }

    }


    public class HLS_Band
    {
        public string Band_Name
        {
            get;
            set;
        }
        public Boolean Check_Status
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class HLSDockpane_ShowButton : Button
    {
        protected override void OnClick()
        {
            HLSDockpaneViewModel.Show();
            
        }
    }
}
