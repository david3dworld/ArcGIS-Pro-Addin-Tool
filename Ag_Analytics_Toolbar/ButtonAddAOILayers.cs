using System;
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

namespace Ag_Analytics_Toolbar
{
    internal class ButtonAddAOILayers : Button
    {
        protected override async void OnClick()
        {
            //Create and use a new filter to view Polygon feature classes in a file GDB.
            //The browse filter is used in an OpenItemDialog.
            BrowseProjectFilter bf = new BrowseProjectFilter
            {
                //Name the filter
                Name = "Polygon feature class in FGDB"
            };
            //Add typeID for Polygon feature class
            bf.AddCanBeTypeId("fgdb_fc_polygon");
            //Allow only File GDBs
            bf.AddDontBrowseIntoFlag(BrowseProjectFilter.FilterFlag.DontBrowseFiles);
            bf.AddDoBrowseIntoTypeId("database_fgdb");
            //Display only folders and GDB in the browse dialog
            bf.Includes.Add("FolderConnection");
            bf.Includes.Add("GDB");
            //Does not display Online places in the browse dialog
            bf.Excludes.Add("esri_browsePlaces_Online");

            //Display the filter in an Open Item dialog
            OpenItemDialog dlg = new OpenItemDialog
            {
                Title = "Open Polygon Feature classes",
                //InitialLocation = Path.GetDirectoryName(Project.Current.URI),
                AlwaysUseInitialLocation = false,
                MultiSelect = true,
                BrowseFilter = bf
            };

            bool? ok = dlg.ShowDialog();

            if (ok == true)
            {
                var items = dlg.Items;

                if (MapView.Active != null)
                {
                    var _map = MapView.Active.Map;
                    await QueuedTask.Run(() =>
                    {
                        foreach (Item item in items)
                        {
                            LayerFactory.Instance.CreateLayer(item, _map);
                        }
                    });

                }
            }
        }
    }
}
