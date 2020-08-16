using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Ag_Analytics_Toolbar
{
    internal class ButtonAddGeoJSON : Button
    {
        protected override void OnClick()
        {
            var parameters = Geoprocessing.MakeValueArray("", "", "POLYGON");
            Geoprocessing.OpenToolDialog("conversion.JSONToFeatures", parameters);

            //await AddGeoJSON();
        }

        private async Task AddGeoJSON()
        {
            //Display the filter in an Open Item dialog
            BrowseProjectFilter bf = new BrowseProjectFilter("esri_browseDialogFilters_browseFiles");
            bf.Name = "GeoJSON";
            bf.FileExtension = "*.geojson";
            bf.BrowsingFilesMode = true;

            OpenItemDialog dlg = new OpenItemDialog
            {
                Title = "Open GeoJSON File",
                //InitialLocation = 
                AlwaysUseInitialLocation = true,
                MultiSelect = false,
                BrowseFilter = bf
            };

            bool? ok = dlg.ShowDialog();

            if (ok == true)
            {
                if (MapView.Active == null)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Map is not active");
                    return;
                }
                else
                {
                    var item = dlg.Items.First();

                    ArcGIS.Desktop.Framework.Threading.Tasks.ProgressDialog progressDialog;
                    progressDialog = new ArcGIS.Desktop.Framework.Threading.Tasks.ProgressDialog("Importing AOI GeoJSON Polygon ...");
                    progressDialog.Show();

                    await ConvertGeoJSONToFeatures(item.Path);

                    progressDialog.Hide();
                }
            }
        }

        private static async Task ConvertGeoJSONToFeatures(string in_geojsonfile)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string featureClassName = "AOI_GeoJSON_Polygon_" + timestamp;
            string output = Path.Combine(Project.Current.DefaultGeodatabasePath, featureClassName);
            
            var parameters = Geoprocessing.MakeValueArray(in_geojsonfile, output, "POLYGON");
            
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("conversion.JSONToFeatures", parameters);

            //return string.IsNullOrEmpty(gpResult.ReturnValue) ? $@"Error in gp tool: {gpResult.ErrorMessages}" : $@"Ok: {gpResult.ReturnValue}";
            if (string.IsNullOrEmpty(gpResult.ReturnValue)){
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($@"Error in gp tool: {gpResult.ErrorMessages}");
            }
            
        }
    }
}
