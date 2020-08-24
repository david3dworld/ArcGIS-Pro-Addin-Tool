using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace Ag_Analytics_Toolbar.Azure_Active_Directory_B2C
{
    internal class ShowMainWindow : Button
    {

        private MainWindow _mainwindow = null;

        protected override void OnClick()
        {
            //already open?
            if (_mainwindow != null)
                return;
            _mainwindow = new MainWindow();
            _mainwindow.Owner = FrameworkApplication.Current.MainWindow;
            _mainwindow.Closed += (o, e) => { _mainwindow = null; };
            _mainwindow.Show();
            //uncomment for modal
            //_mainwindow.ShowDialog();
        }

    }
}
