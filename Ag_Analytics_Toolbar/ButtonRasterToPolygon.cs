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
    internal class ButtonRasterToPolygon : Button
    {
        protected override void OnClick()
        {
            string input_raster = "";
            //string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            //string featureClassName = "AOI_Raster_Polygon_" + timestamp;
            //string output_feature = Path.Combine(Project.Current.DefaultGeodatabasePath, featureClassName);
            string output = "";
            var parameters = Geoprocessing.MakeValueArray(input_raster, output, "POLYGON");
            
            Geoprocessing.OpenToolDialog("3d.RasterDomain", parameters);
        }
    }
}
