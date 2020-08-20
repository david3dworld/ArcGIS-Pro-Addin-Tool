# ArcGIS-Pro-Addin-Tool



### Table of Contents

- [1. General workflow on creating a new tool in the add-in. and description of the add-in's framework.](#1.-General-workflow-on-creating-a-new-tool-in-the-add-in.-and-description-of-the-add-in's-framework.)
- [2. Installation](#2.-Installation.)
- [3. How to add toolbar on arcgispro](#3.-how-to-add-toolbar-on-arcgispro)
- [Usage](#usage)
- [4. main function details for the OAuth login and APIs.](#2.-main-function-details-for-the-OAuth-login-and-APIs.)
- [Contributing](#contributing)


## 1. General workflow on creating a new tool in the add-in. and description of the add-in's framework.

1) Allow user to make requests and display/save geospatial results from several custom
APIs. In general the user will define an AOI (either draw, select file, or click boundary from
base layer) of a farm field and then (depending on the API, select some preset options)
and then retrieve the result from the API. 

The APIs will return JSON with links to download
either shape/raster or other files.

2) User Authentication thru our AzureB2C Tenant using Oauth2. 

A window should pop open
to allow them to log into our web app and create an account. 

The add in will then
authenticate against our backend web app and expose certain APIs to download user
data. The APIs will be provided. 

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/General.png)

The contractor  establish the basic authentication framework.

Broadly, the add-in can be to use oauth2 to log in to our app within
ArcGIS Pro/ArcGIS online (Esri documentation) as well as our Web Application(which uses Azure B2C).

After the add-in model is installed in ArcGIS Pro, a pop-up browser window immediately be available for the user to create an account at our websiteand log in.

Then authenticate the user through the ArcGIS Pro or
Desktop clients to our app in the same way they would if someone logged into our website through a browser or used the mobile app. So, ArcGIS in this case will be the front end.

Once logged in, the user can gain access to all functions in the add-in module
and request data the same way they could, if they were logged in to our site. 

The APIs  deliver data back to them such as shapefiles, rasters, operation tables,etc.
 Additionally, through the add-in,  user can easily access our utilities and data
Users can access a search pane/engine from the tabs section that
provides a dropdown of all available data sets(shapefiles & rasters), to help them select the data that they want to add to their analysis.

Users can access a help tab that provides an external link Help and
API documentation pages and allows the user to submit a request for
assistance.

Users can access the add-in content through an ArcGIS tab. 

By selecting a specific API from the menu (refer to the concept tool below) and
passing user’s data (ex: geojson, rasters, shapefile within a zipfile), parameters
for the API, visualization options, and working directory.

The user can option to either download files into
filestorage, geodatabase, arcgis enterprise, online and other standard arcgis
options.

The results are generally are in three data types: rasters(.tif), shapefiles (.shp,
.shx. Etc. in a zip file), and json string contents the data information. Users can visualize the results rasters and shapefiles in ArcGIS Pro Map
Display, and json results in a table or interactive plots.

The figure below shows some of the functionality the contractor will integrate to
ArcGIS through the Add-in.

## 2.Installation.

> 1) ArcGIS Pro 2.6 install
[ArcGIS Pro 2.6 install](https://pro.arcgis.com/en/pro-app/get-started/install-and-sign-in-to-arcgis-pro.htm)
> 2) add toolbar on arcgis pro
[Intructions](https://awesomeopensource.com/project/Esri/arcgis-pro-sdk-community-samples/)
> 3) Visual studio 2019 install (for developers)
[Visual Studio 2019 Download](https://visualstudio.microsoft.com/downloads/)
> 4) .Net framework4.8 (for developers)
[.Net framework4.8 Download](https://dotnet.microsoft.com/download/)


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

Broadly, the add-in can use oauth2 to log in to our app within
ArcGIS Pro/ArcGIS online (Esri documentation) as well as our Web Application
(which uses Azure B2C).

After the add-in model is installed in ArcGIS Pro, a pop-up browser window  can  immediately be available for the user to create an account at our website
and log in. 

This should then authenticate the user through the ArcGIS Pro or
Desktop clients to our app in the same way they would if someone logged into our website through a browser or used the mobile app. 

So, ArcGIS in this case will be the front end.
Once logged in, the user  gain access to all functions in the add-in module and request data the same way they could, 
if they were logged in to our site. 

The APIs will deliver data back to them such as shapefiles, rasters, operation tables,etc.

Additionally, through the add-in, user to easily access our utilities and data APIs (we will provide those APIs):
Users can access a search pane/engine from the tabs section that
provides a dropdown of all available data sets(shapefiles & rasters), to help them
select the data that they want to add to their analysis.

Users can access a help tab that provides an external link Help and
API documentation pages and allows the user to submit a request for
assistance.

Users can access the add-in content through an ArcGIS tab. 

By selecting a specific API from the menu (refer to the concept tool below) and passing user’s data 
(ex: geojson, rasters, shapefile within a zipfile), parameters
for the API, visualization options, and working directory.

The add-in can check if the parameters put in by the user meet the
API requirements (Data format, missing parameters, etc.). 

If requirements are not met, a warning message should be sent back to users. Then the add-in will pass the data and parameters to our APIs. 

The API response  return the download URL for the result data layer (shapefiles and rasters). 

After that, the add-in  download the data to the user's working directory and display the
results in ArcGIS Pro. 

The user can option to either download files into
filestorage, geodatabase, arcgis enterprise, online and other standard arcgis options.

The results are generally are in three data types: rasters(.tif), shapefiles (.shp,
.shx. Etc. in a zip file), and json string contents the data information. Users can visualize the results rasters and shapefiles in ArcGIS Pro Map
Display, and json results in a table or interactive plots.

The figure below shows some of the functionality the contractor will integrate to
ArcGIS through the Add-in.



