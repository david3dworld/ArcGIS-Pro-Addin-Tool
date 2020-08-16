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
    internal class ButtonCreateAOILayer : Button
    {
        protected override async void OnClick()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string featureClassName = "AOI_Polygon_" + timestamp;

            List<object> arguments = new List<object>
            {
                // store the results in the default geodatabase
                CoreModule.CurrentProject.DefaultGeodatabasePath,
                // name of the feature class
                featureClassName,
                // type of geometry
                "POLYGON",  //POINT,POLYLINE,POLYGON...
                // no template
                "",
                // no z values
                "DISABLED",
                // no m values
                "DISABLED"
            };
            
            await QueuedTask.Run(() =>
            {
                // spatial reference
                arguments.Add(SpatialReferenceBuilder.CreateSpatialReference(4326)); //3857
            });

            var parameters = Geoprocessing.MakeValueArray(arguments.ToArray());
            
            Geoprocessing.OpenToolDialog("management.CreateFeatureclass", parameters);

            //await CreateAOILayer();
        }
        
        private async Task CreateAOILayer()
        {
            if (MapView.Active == null)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Map is not active");
                return;
            }
            ArcGIS.Desktop.Framework.Threading.Tasks.ProgressDialog createAOIDialog;
            createAOIDialog = new ArcGIS.Desktop.Framework.Threading.Tasks.ProgressDialog("Creating new AOI Polygon ...");
            createAOIDialog.Show();
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string featureClassName = "AOI_Polygon_" + timestamp;
            await CreateFeatureClass(featureClassName, "POLYGON");
            // select the type of tool feedback you wish to implement.  

            createAOIDialog.Hide();
        }

        private static async Task CreateFeatureClass(string featureclassName, string featureclassType)
        {
            List<object> arguments = new List<object>
            {
                // store the results in the default geodatabase
                CoreModule.CurrentProject.DefaultGeodatabasePath,
                // name of the feature class
                featureclassName,
                // type of geometry
                featureclassType,  //POINT,POLYLINE,POLYGON...
                // no template
                "",
                // no z values
                "DISABLED",
                // no m values
                "DISABLED"
            };

            await QueuedTask.Run(() =>
            {
                // spatial reference
                arguments.Add(SpatialReferenceBuilder.CreateSpatialReference(4326)); //3857
            });
            
            IGPResult result = await Geoprocessing.ExecuteToolAsync("CreateFeatureclass_management", Geoprocessing.MakeValueArray(arguments.ToArray()));
        }
    }
}
