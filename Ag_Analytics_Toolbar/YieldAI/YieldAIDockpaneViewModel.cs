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

namespace Ag_Analytics_Toolbar.YieldAI
{
    internal class YieldAIDockpaneViewModel : DockPane
    {
        private const string _dockPaneID = "Ag_Analytics_Toolbar_YieldAI_YieldAIDockpane";
        
        private ObservableCollection<FeatureLayer> _polygonFeatureLayers = new ObservableCollection<FeatureLayer>();
        private FeatureLayer _selectedPolygonFeatureLayer = null;

        private ObservableCollection<string> _modelNames = new ObservableCollection<string>();
        private string _selectedModelName = "";

        private ObservableCollection<string> _cropSeasons = new ObservableCollection<string>();
        private string _selectedCropSeason = "";

        private DateTime _plantingDay1 = DateTime.Now;
        private DateTime _harvestDay = DateTime.Now;

        private int _seedingDensity = 30000;

        private string _downloadPath = "";

        private readonly ICommand _zoomToLayerCommand;
        public ICommand ZoomToLayerCommand => _zoomToLayerCommand;

        private readonly ICommand _downloadPathCommand;
        public ICommand DownloadPathCommand => _downloadPathCommand;

        private readonly ICommand _submitCommand;
        public ICommand SubmitCommand => _submitCommand;
        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;

        protected YieldAIDockpaneViewModel() {
            string[] model_names = {"NN"};
            foreach (string model_name in model_names)
            {
                _modelNames.Add(model_name);
            }
            _selectedModelName = _modelNames.First();
            for (int i = 2013; i <= DateTime.Now.Year; i++)
            {
                _cropSeasons.Add(i.ToString());
            }
            _selectedCropSeason = _cropSeasons.Last();

            _zoomToLayerCommand = new RelayCommand(() => ZoomToLayerExecute(), () => true);
            _downloadPathCommand = new RelayCommand(() => DownloadPathExecute(), () => true);
            _submitCommand = new RelayCommand(() => SubmitExecute(), () => true);
            _cancelCommand = new RelayCommand(() => CancelExecute(), () => true);

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
        private string _heading = "YieldAI Parameters";
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
            System.Diagnostics.Process.Start(@"https://ag-analytics.portal.azure-api.net/docs/services/yield-forecast/operations/yield-model");
        }

        public void CancelExecute()
        {
            DockPane pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane == null)
                return;

            pane.Hide();
        }

        protected override Task InitializeAsync()
        {
            ActiveMapViewChangedEvent.Subscribe(OnActiveMapViewChangedEvent);

            LayersAddedEvent.Subscribe(OnLayersAddedEvent);
            LayersRemovedEvent.Subscribe(OnLayersRemovedEvent);

            return base.InitializeAsync();
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
            SetPolygonFeatureLayers();
        }

        private void OnLayersRemovedEvent(LayerEventsArgs args)
        {
            SetPolygonFeatureLayers();
        }

        public async void ZoomToLayerExecute()
        {
            if (MapView.Active != null)
            {
                if (_selectedPolygonFeatureLayer != null)
                {
                    await MapView.Active.ZoomToAsync(_selectedPolygonFeatureLayer);
                }
            }
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
            get { return _selectedPolygonFeatureLayer; }

            set
            {
                SetProperty(ref _selectedPolygonFeatureLayer, value, () => SelectedPolygonFeatureLayer);
            }
        }

        public ObservableCollection<string> ModelNames
        {
            get { return _modelNames; }
            set
            {
                SetProperty(ref _modelNames, value, () => ModelNames);
            }
        }

        public string SelectedModelName
        {
            get { return _selectedModelName; }
            set
            {
                SetProperty(ref _selectedModelName, value, () => SelectedModelName);
            }
        }

        public ObservableCollection<string> CropSeasons
        {
            get { return _cropSeasons; }
            set
            {
                SetProperty(ref _cropSeasons, value, () => CropSeasons);
            }
        }

        public string SelectedCropSeason
        {
            get { return _selectedCropSeason; }
            set
            {
                SetProperty(ref _selectedCropSeason, value, () => SelectedCropSeason);
            }
        }

        public DateTime PlantingDay1
        {
            get { return _plantingDay1; }
            set
            {
                SetProperty(ref _plantingDay1, value, () => PlantingDay1);
            }
        }

        public DateTime HarvestDay
        {
            get { return _harvestDay; }
            set
            {
                SetProperty(ref _harvestDay, value, () => HarvestDay);
            }
        }

        public int SeedingDensity
        {
            get { return _seedingDensity; }
            set
            {
                SetProperty(ref _seedingDensity, value, () => SeedingDensity);
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

        private void SetPolygonFeatureLayers()
        {
            try
            {
                // Gets the first 2D map from the project that is called Map.
                //Map _map = await GetMapFromProject(Project.Current, "Map");
                if (MapView.Active == null)
                {
                    //ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("SetPolygonFeatureLayers: map is not active.");
                    return;
                }

                var _map = MapView.Active.Map;

                List<FeatureLayer> featureLayerList = _map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();

                ObservableCollection<FeatureLayer> newPolygonFeatureLayers = new ObservableCollection<FeatureLayer>();

                foreach (FeatureLayer featureLayer in featureLayerList)
                {
                    if (featureLayer.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        newPolygonFeatureLayers.Add(featureLayer);
                    }
                }
                PolygonFeatureLayers = newPolygonFeatureLayers;
                if (PolygonFeatureLayers.Count > 0)
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
                var client = new RestClient("https://ag-analytics.azure-api.net/yieldforecast/");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);

                //request.AlwaysMultipartFormData = true;
                //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("MODELNAME", _selectedModelName);
                request.AddParameter("SHAPE", aoi);

                var scalarVariables = new
                {
                    CropSeason = _selectedCropSeason,
                    PlantingDay1 = String.Format("{0:M/d/yyyy}", _plantingDay1),
                    HarvestDay = String.Format("{0:M/d/yyyy}", _harvestDay),
                    SeedingDensity = _seedingDensity
                };

                string scalarVariables_string = JsonConvert.SerializeObject(scalarVariables);
                
                request.AddParameter("ScalarVariables", scalarVariables_string);

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
                    string filename = jsonData.raster_filename;

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
                var download_client = new RestClient("https://ag-analytics.azure-api.net/yieldforecast/?filename=" + filename);
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

                    await Ag_Analytics_Module.SetToClassifyColorizerFromLayerName(rasterName, 10, "Slope");

                    // delete files in default path
                    File.Delete(fullPath);
                }
                catch
                {
                    await DownloadFile(download_path, filename);

                    await Ag_Analytics_Module.AddRasterLayerToMapAsync(Path.Combine(download_path, filename));

                    await Ag_Analytics_Module.SetToClassifyColorizerFromLayerName(filename, 10, "Slope");
                }
            });
        }
    }

    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class YieldAIDockpane_ShowButton : Button
    {
        protected override void OnClick()
        {
            YieldAIDockpaneViewModel.Show();
        }
    }
}
