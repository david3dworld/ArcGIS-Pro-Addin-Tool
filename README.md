# ArcGIS-Pro-Addin-Tool



### Table of Contents

- [1. General workflow on creating a new tool in the add-in. and description of the add-in's framework.](#1.-General-workflow-on-creating-a-new-tool-in-the-add-in.-and-description-of-the-add-in's-framework.)
- [2. Installation](#2.-Installation.)
- [3. How to add toolbar on arcgispro](#3.-how-to-add-toolbar-on-arcgispro)
- [Usage](#usage)
- [4. main function details for the OAuth login and APIs.](#2.-main-function-details-for-the-OAuth-login-and-APIs.)
- [Contributing](#contributing)


## 1. General workflow on creating a new tool in the add-in. and description of the add-in's framework.

It is a system that allows you to submit and download various APIs provided by Ag-Analytics in ArcGIS pro 2.6.

Ag-Analytics Farm Management Platform provides a wide range of services and data products with precision field data integration for farmers, retailers, suppliers and more.

HLS (Specially Harmonized Landsat Sentinel) service, YieldAI, DEM (Digital Elevation Model) service, BoundaryAI, Cropland Data Layers, ProfitLayer, ADAPT, etc.

This is a system that allows users to view and receive data using the Toolbar in the ArcGIS Pro environment.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Generalworkflow.png)

## 2.Installation.

To use the ArcGIS pro Toolbar function, the following environments must be installed.
> 1) ArcGIS Pro 2.6 install
[ArcGIS Pro 2.6 install](https://pro.arcgis.com/en/pro-app/get-started/install-and-sign-in-to-arcgis-pro.htm)
> 2) .Net framework4.8 (for developers)
[.Net framework4.8 Download](https://dotnet.microsoft.com/download/)
> 3) Visual studio 2019 install (for developers)
[Visual Studio 2019 Download](https://visualstudio.microsoft.com/downloads/)
> 4) How to add toolbar on arcgis pro
[Intructions](https://awesomeopensource.com/project/Esri/arcgis-pro-sdk-community-samples/) 

If you have already registered anything, just uninstall it and install it again with new version.

## How to add toolbar on arcgispro

[Quick Access Toolbar](https://pro.arcgis.com/en/pro-app/get-started/quick-access-toolbar.htm)

> How to add a toolbox in ArcGIS pro.


To add a toolbox to your project, click Insert > Toolbox > Add Toolbox and browse to the toolbox, or right-click any toolbox in a folder connection and select Add To Project. 

A reference to the toolbox will be saved with your project in the Toolboxes node in the Catalog pane.

You can share add-ins easily because they do not require an additional installation program. 
You can add add-ins to your machine from a shared network location by double-clicking the file in Windows File Explorer or by manually copying the file to a well-known folder. 

Add-ins can also be shared by email or from a portal.


> Note :
Add-ins are developed with the ArcGIS Pro SDK 2.6 for the Microsoft .NET Framework. 

Visit the ArcGIS for Developers site to build your first add-in.

### Validate and copy an add-in

1. Before you install an add-in, it must be validated. 

Validation ensures that the file is copied to the appropriate location and that there are no name conflicts. 

It also guarantees that an existing version of the add-in is not overwritten by an older version.

2. Locate the add-in on your computer or shared network drive.

You may browse to its location using Windows File Explorer, open it from an email attachment, or download it from a portal.

3. Double-click the add-in.

The Esri ArcGIS Add-In Installation Utility opens.

4. On the Esri ArcGIS Add-In Installation Utility, review the author, description, version, and digital signature information of the add-in.

Click Install Add-In.

The add-in is validated and copied to a well-known folder for use in ArcGIS Pro.

### View add-ins

Once an add-in has been validated and installed, you can view it in the Add-In Manager and use it in ArcGIS Pro. To open the Add-In Manager options, follow these steps:

1. From an open project, click the Project tab on the ribbon. 

Alternatively, on the ArcGIS Pro start page, click Settings Settings in the lower left corner.

2. In the list on the left, click Add-In Manager.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/GUID-573CBC5C-DE1D-4843-B45F-D5772F380C95-web.png)

3. When you open the Add-In Manager page, ArcGIS Pro searches specified well-known folders for add-in files. After the files are located, they are installed and appear on the Add-Ins tab of the Add-In Manager. 

The Add-Ins tab provides information about available add-ins.



### Delete an add in 

You can delete local add-ins from the Add-In Manager. 

Deleting an add-in moves the file from its system folder to the system recycle bin. 

Shared add-ins are listed under Shared Add-ins and cannot be deleted through the Add-In Manager.

> 1. On the Add-In Manager, ensure that the Add-Ins tab is selected. 

Under My Add-Ins, click the add-in you want to delete.

Information about the add-in is displayed.

> 2. In the lower right corner of the screen, click Delete this Add-In.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/GUID-E51E3808-2E6F-41BD-B27A-C94B7CFF603E-web.png)

The add-in is marked with a message that it has been deleted. 

It is still visible in the Add-In Manager and is available in the current ArcGIS Pro session.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/GUID-20B05D95-6002-4D97-B70D-271710F5D6B6-web.png)


## Usage

1) How to login.

2) How to use HLS service

3) How to use DEM service

4) How to use Yield service





