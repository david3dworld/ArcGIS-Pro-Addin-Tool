using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

using System.IO;
using System.Web;
using System.Net;

using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Core.Geoprocessing;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ag_Analytics_Toolbar
{
    internal class Ag_Analytics_Module : Module
    {
        private static Ag_Analytics_Module _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Ag_Analytics_Module Current
        {
            get
            {
                return _this ?? (_this = (Ag_Analytics_Module)FrameworkApplication.FindModule("Ag_Analytics_Toolbar_Module"));
            }
            
        }

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides
        public static async Task<string> GetGeoJSONFromFeatureLayer(FeatureLayer polygonFeatureLayer)
        {
            return await QueuedTask.Run(() => {
                
                QueryFilter filter = new QueryFilter
                {
                    WhereClause = "1=1"
                };

                RowCursor rowCursor = polygonFeatureLayer.Search(filter);
                
                List<object> _features = new List<object>();

                while (rowCursor.MoveNext())
                {
                    var row = rowCursor.Current;
                    Feature feature = row as Feature;

                    Polygon polygon = feature.GetShape() as Polygon;

                    IReadOnlyList<Coordinate2D> pts = polygon.Copy2DCoordinatesToList();

                    List<object> _coordinates = new List<object>();
                    List<object> _main_coordinates = new List<object>();
                    //List<object> _hole_coordinates = new List<object>(); // ignore hole coordinates

                    foreach (var pt in pts)
                    {
                        double[] _pt = { pt.X, pt.Y };
                        _main_coordinates.Add(_pt);
                    }
                    _coordinates.Add(_main_coordinates);

                    var _geometry = new
                    {
                        type = "Polygon",
                        coordinates = _coordinates
                    };
                    var _properties = new
                    {
                        //OBJECTID =
                    };
                    var _feature = new
                    {
                        type = "Feature",
                        geometry = _geometry,
                        properties = _properties
                    };

                    _features.Add(_feature);
                }

                var geojson_object = new
                {
                    type = "FeatureCollection",
                    features = _features
                };

                string geojson_string = JsonConvert.SerializeObject(geojson_object.features[0]);

                return geojson_string;
            });
        }

        public static async Task ConvertToGeodatabase(string input_raster, string gdb)
        {
            var parameters = Geoprocessing.MakeValueArray(input_raster, gdb);
            IGPResult result = await Geoprocessing.ExecuteToolAsync("conversion.RasterToGeodatabase", parameters);
        }
        
        public static async Task CopyRaster(string input_raster, string output_raster)
        {
            var parameters = Geoprocessing.MakeValueArray(input_raster, output_raster);
            IGPResult result = await Geoprocessing.ExecuteToolAsync("management.CopyRaster", parameters);
        }

        public static async Task CopyFeatures(string input, string output)
        {
            var parameters = Geoprocessing.MakeValueArray(input, output);
            IGPResult result = await Geoprocessing.ExecuteToolAsync("management.CopyFeatures", parameters);
        }

        public static async Task CopyTable(string input, string output, string tableName)
        {
            var parameters = Geoprocessing.MakeValueArray(input, output, tableName);
            IGPResult result = await Geoprocessing.ExecuteToolAsync("conversion.TableToTable", parameters);
        }

        public static async Task RasterDomain(string input_raster)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string featureClassName = "AOI_Raster_Polygon_" + timestamp;

            string output = Path.Combine(Project.Current.DefaultGeodatabasePath, featureClassName);

            var parameters = Geoprocessing.MakeValueArray(input_raster, output, "POLYGON");

            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("ddd.RasterDomain", parameters);
        }

        public static async Task AddLayerToMapAsync(string dataSourceUrl)
        {
            try
            {
                if (MapView.Active == null)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("AddLayer: map is not active.");
                    return;
                }

                var _map = MapView.Active.Map;

                await QueuedTask.Run(() =>
                {
                    LayerFactory.Instance.CreateLayer(new Uri(@dataSourceUrl), _map);
                });
            }
            catch (Exception exc)
            {
                // Catch any exception found and display a message box.
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception caught while trying to add layer: " + exc.Message);
                return;
            }
        }

        public static Task<Map> GetMapFromProject(Project project, string mapName)
        {
            // Return null if either of the two parameters are invalid.
            if (project == null || string.IsNullOrEmpty(mapName))
                return null;

            // Find the first project item with name matches with mapName
            MapProjectItem mapProjItem =
                project.GetItems<MapProjectItem>().FirstOrDefault(item => item.Name.Equals(mapName, StringComparison.CurrentCultureIgnoreCase));

            if (mapProjItem != null)
                return QueuedTask.Run<Map>(() => { return mapProjItem.GetMap(); }, Progressor.None);
            else
                return null;
        }

        public static async Task SetToStretchColorizerFromLayerName(string layerName, string colorRampName)
        {
            try
            {
                // Gets the first 2D map from the project that is called Map.
                //Map _map = await GetMapFromProject(Project.Current, "Map");
                if (MapView.Active == null)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed to get map.");
                    return;
                }

                var _map = MapView.Active.Map;

                // Gets the selected layers from the current Map.
                IReadOnlyList<Layer> selectedLayers = _map.FindLayers(layerName);

                if (selectedLayers.Count == 1)
                {
                    // Gets the selected layer.
                    Layer firstSelectedLayer = selectedLayers.First();

                    if (firstSelectedLayer != null && (firstSelectedLayer is BasicRasterLayer || firstSelectedLayer is MosaicLayer))
                    {
                        // Gets the basic raster layer from the selected layer. 

                        BasicRasterLayer basicRasterLayer = null;

                        if (firstSelectedLayer is BasicRasterLayer)
                            basicRasterLayer = (BasicRasterLayer)firstSelectedLayer;
                        else if (firstSelectedLayer is MosaicLayer)
                            basicRasterLayer = ((MosaicLayer)firstSelectedLayer).GetImageLayer() as BasicRasterLayer;

                        await SetToStretchColorizerFromLayer(basicRasterLayer, colorRampName);
                    }
                }
            }
            catch (Exception exc)
            {
                // Catch any exception found and display a message box.
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception caught while trying to get layer: " + exc.Message);
                return;
            }
        }

        private static async Task SetToStretchColorizerFromLayer(BasicRasterLayer basicRasterLayer, string colorRampName)
        {
            // Defines parameters in colorizer definition.
            int bandIndex = 0;
            RasterStretchType stretchType = RasterStretchType.MinimumMaximum;
            double gamma = 1.0;
            string colorRampStyle = "ArcGIS Colors";
            
            await QueuedTask.Run(async () =>
            {
                // Gets a color ramp from a style.
                IList<ColorRampStyleItem> rampList = GetColorRampsFromStyleAsync(Project.Current, colorRampStyle, colorRampName);
                CIMColorRamp colorRamp = rampList[0].ColorRamp;

                // Creates a new Stretch Colorizer Definition with defined parameters.
                StretchColorizerDefinition stretchColorizerDef = new StretchColorizerDefinition(bandIndex, stretchType, gamma, colorRamp);

                // Creates a new stretch colorizer using the colorizer definition created above.
                CIMRasterStretchColorizer newColorizer = await basicRasterLayer.CreateColorizerAsync(stretchColorizerDef) as CIMRasterStretchColorizer;

                // Sets the newly created colorizer on the layer.
                basicRasterLayer.SetColorizer(newColorizer);

            });

        }

        public static async Task SetToClassifyColorizerFromLayerName(string layerName, int numberofClasses, string colorRampName)
        {
            try
            {
                // Gets the first 2D map from the project that is called Map.
                //Map _map = await GetMapFromProject(Project.Current, "Map");
                if (MapView.Active == null)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed to get map.");
                    return;
                }

                var _map = MapView.Active.Map;

                // Gets the selected layers from the current Map.
                IReadOnlyList<Layer> selectedLayers = _map.FindLayers(layerName);

                if (selectedLayers.Count == 1)
                {
                    // Gets the selected layer.
                    Layer firstSelectedLayer = selectedLayers.First();

                    if (firstSelectedLayer != null && (firstSelectedLayer is BasicRasterLayer || firstSelectedLayer is MosaicLayer))
                    {
                        // Gets the basic raster layer from the selected layer. 

                        BasicRasterLayer basicRasterLayer = null;

                        if (firstSelectedLayer is BasicRasterLayer)
                            basicRasterLayer = (BasicRasterLayer)firstSelectedLayer;
                        else if (firstSelectedLayer is MosaicLayer)
                            basicRasterLayer = ((MosaicLayer)firstSelectedLayer).GetImageLayer() as BasicRasterLayer;

                        await SetToClassifyColorizerFromLayer(basicRasterLayer, numberofClasses, colorRampName);
                    }
                }
            }
            catch (Exception exc)
            {
                // Catch any exception found and display a message box.
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Exception caught while trying to get layer: " + exc.Message);
                return;
            }
        }

        private static async Task SetToClassifyColorizerFromLayer(BasicRasterLayer basicRasterLayer, int numberofClasses, string colorRampName)
        {
            // Defines values for parameters in colorizer definition.
            string fieldName = "Value";
            ClassificationMethod classificationMethod = ClassificationMethod.EqualInterval;
            string colorRampStyle = "ArcGIS Colors";

            await QueuedTask.Run(async () =>
            {
                // Gets a color ramp from a style.
                IList<ColorRampStyleItem> rampList = GetColorRampsFromStyleAsync(Project.Current, colorRampStyle, colorRampName);
                CIMColorRamp colorRamp = rampList[0].ColorRamp;

                // Creates a new Classify Colorizer Definition using defined parameters.
                ClassifyColorizerDefinition classifyColorizerDef = new ClassifyColorizerDefinition(fieldName, numberofClasses, classificationMethod, colorRamp);

                // Creates a new Classify colorizer using the colorizer definition created above.
                CIMRasterClassifyColorizer newColorizer = await basicRasterLayer.CreateColorizerAsync(classifyColorizerDef) as CIMRasterClassifyColorizer;

                // Sets the newly created colorizer on the layer.
                basicRasterLayer.SetColorizer(newColorizer);
            });
        }

        private static IList<ColorRampStyleItem> GetColorRampsFromStyleAsync(Project project, string styleName, string rampName)
        {
            try
            {
                // Gets a collection of style items in project that matches the given style name.
                StyleProjectItem styleItem =
                project.GetItems<StyleProjectItem>().FirstOrDefault(style => (style.Name.Equals(styleName, StringComparison.CurrentCultureIgnoreCase)));

                if (styleItem == null)
                    throw new System.ArgumentNullException();

                //Search for color ramp by name in the collection of style items.
                return styleItem.SearchColorRamps(rampName);
                
            }
            catch (Exception ex)
            {

                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(null, $@"Error in GetColorRampsFromStyleAsync: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }

    }
}
