# ArcGIS-Pro-Addin-Tool



### Table of Contents

- [1. General workflow on creating a new tool in the add-in. and description of the add-in's framework.](#1.-General-workflow-on-creating-a-new-tool-in-the-add-in.and-description-of-the-add-in's-framework.)
- [2. Installation](#2.-Installation.)
- [3. How to add toolbar on arcgispro](#3.-how-to-add-toolbar-on-arcgispro)
- [Usage](#usage)
- [4. main function details for the OAuth login and APIs.](#2.-main-function-details-for-the-OAuth-login-and-APIs.)
- [Contributing](#contributing)


## Toolbar Platform
This is ...

## 2.Installation.

> 1) Visual studio 2019 install
[Visual Studio 2019 Download](https://visualstudio.microsoft.com/downloads/)
> 2) .Net framework4.8
[.Net framework4.8 Download](https://dotnet.microsoft.com/download/)
> 3) add toolbar on arcgis pro
[Intructions](https://awesomeopensource.com/project/Esri/arcgis-pro-sdk-community-samples/)


## How to add toolbar on arcgispro

[Quick Access Toolbar](https://pro.arcgis.com/en/pro-app/get-started/quick-access-toolbar.htm)

> How to add a toolbox in ArcGIS pro.


To add a toolbox to your project, click Insert > Toolbox > Add Toolbox and browse to the toolbox, or right-click any toolbox in a folder connection and select Add To Project. 
A reference to the toolbox will be saved with your project in the Toolboxes node in the Catalog pane.

You can share add-ins easily because they do not require an additional installation program. 
You can add add-ins to your machine from a shared network location by double-clicking the file in Windows File Explorer or by manually copying the file to a well-known folder. 
Add-ins can also be shared by email or from a portal.

> Note :
Add-ins are developed with the ArcGIS Pro SDK 2.6 for the Microsoft .NET Framework. Visit the ArcGIS for Developers site to build your first add-in.

### Validate and copy an add-in

1. Before you install an add-in, it must be validated. Validation ensures that the file is copied to the appropriate location and that there are no name conflicts. It also guarantees that an existing version of the add-in is not overwritten by an older version.

2. Locate the add-in on your computer or shared network drive.
You may browse to its location using Windows File Explorer, open it from an email attachment, or download it from a portal.

3. Double-click the add-in.
The Esri ArcGIS Add-In Installation Utility opens.

4. On the Esri ArcGIS Add-In Installation Utility, review the author, description, version, and digital signature information of the add-in.
Click Install Add-In.
The add-in is validated and copied to a well-known folder for use in ArcGIS Pro.

### View add-ins

Once an add-in has been validated and installed, you can view it in the Add-In Manager and use it in ArcGIS Pro. To open the Add-In Manager options, follow these steps:

1. From an open project, click the Project tab on the ribbon. Alternatively, on the ArcGIS Pro start page, click Settings Settings in the lower left corner.

2. In the list on the left, click Add-In Manager.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/GUID-573CBC5C-DE1D-4843-B45F-D5772F380C95-web.png)

3. When you open the Add-In Manager page, ArcGIS Pro searches specified well-known folders for add-in files. After the files are located, they are installed and appear on the Add-Ins tab of the Add-In Manager. The Add-Ins tab provides information about available add-ins.



### Delete an add in 

You can delete local add-ins from the Add-In Manager. Deleting an add-in moves the file from its system folder to the system recycle bin. 
Shared add-ins are listed under Shared Add-ins and cannot be deleted through the Add-In Manager.

> 1. On the Add-In Manager, ensure that the Add-Ins tab is selected. Under My Add-Ins, click the add-in you want to delete.
Information about the add-in is displayed.

> 2. In the lower right corner of the screen, click Delete this Add-In.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/GUID-E51E3808-2E6F-41BD-B27A-C94B7CFF603E-web.png)

The add-in is marked with a message that it has been deleted. It is still visible in the Add-In Manager and is available in the current ArcGIS Pro session.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/GUID-20B05D95-6002-4D97-B70D-271710F5D6B6-web.png)


## Usage

In ArcGIS Pro, a body of related work—consisting of multiple maps, scenes, layouts, data, tables, tools, and other resources—is typically organized in a project. By default, a project is stored in its own system folder. Project files have the extension .aprx. A project also has its own geodatabase (a file with the extension .gdb) and its own toolbox (a file with the extension .tbx).

When you start ArcGIS Pro, there are various ways to open saved projects and create new ones. If you work frequently with a project, you can pin it to the start page for quick access; any project you have recently saved is also accessible from the start page. You can browse to other saved projects to open them.

You can create projects from one of the four system templates. Each template creates a project file and adds content to it. For example, a project created from the Map template starts with a map view containing a basemap layer. You can also start without a template. This allows you to work in ArcGIS Pro without saving a project file.

New projects can also be started from project templates made by you or shared with you by colleagues. A project template is a customized starting state for a project. Recently used templates appear on the start page. You can also browse to templates.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Usage-1.png)

### ArcGIS Pro user interface
The main parts of the ArcGIS Pro interface are the ribbon, views, and panes. For a hands-on introduction, try the Introducing ArcGIS Pro quick-start tutorial.

### Ribbon
ArcGIS Pro uses a horizontal ribbon at the top of the application window to display and organize functionality into a series of tabs. Some of these tabs (core tabs) are always present. Others (contextual tabs) appear when the application is in a particular state. For example, a set of contextual Feature Layer tabs appears when a feature layer is selected in the Contents pane.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Usage-2.png)

You can customize the ribbon and the Quick Access Toolbar.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Usage-3.png)

### Views
Views are windows for working with maps, scenes, tables, layouts, charts, reports, and other presentations of data. A project may have many views, which can be opened and closed as needed. Several views can be open at the same time, but only one is active. The active view affects which tabs appear on the ribbon and which elements are displayed in panes, such as the Contents pane.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Usage-4.png)

### Panes
A pane is a dockable window that displays the contents of a view (the Contents pane), the contents of a project or portal (the Catalog pane), or commands and settings related to an area of functionality, such as the Symbology and Geoprocessing panes.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Usage-5.png)

Panes offer functionality that is more advanced or complete than ribbon commands. Panes may have rows of text tabs and graphical tabs that partition and organize functionality.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Usage-6.png)

The first time you open ArcGIS Pro, the Contents and Catalog panes are open, and all other panes are closed. If you've used ArcGIS Pro before, the same panes that were open during your last session remain open the next time you start the program.

You can manage panes on the ribbon, on the View tab, in the Windows group. Click Contents Contents or Catalog Pane Catalog Pane to reopen these panes if you close them. Click Reset Panes Reset Panes to choose a specific pane configuration.

### Settings page
On the Settings page, you can modify ArcGIS Pro options, configure your license and portal connections, manage add-ins, and more. The Settings page can be accessed in two ways:

From an open project, click the Project tab on the ribbon.
From the ArcGIS Pro start page, click Settings in the lower left corner.
You can access the settings you want to change using the list on the left side of the page. You can also create, open, and save projects; open the help system; get information about ArcGIS Pro; and exit the application.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Usage-7.png)



```C#
import ()
This is...
```
=======
'''C#
import ()
This is...
'''


## Contributing
this is testing readme file

Please test

## License

[MIT](http://google.com)

