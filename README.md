# ArcGIS-Pro-Add-in-Tool



### Table of Contents

- [1. Overview.](#1.-Overview.)
- [2. Installation](#2.-Installation.)
- [3. Usage](#3.-Usage)


## 1. Overview.

This program is a system that allows you to submit and download various useful geographic data from many kinds of APIs provided by Ag-Analytics services on ArcGIS Pro.

Ag-Analytics Farm Management Platform provides a wide range of services and data products with precision field data integration for farmers, retailers, suppliers and more.

For example, HLS (Specially Harmonized Landsat Sentinel) service, YieldAI, DEM (Digital Elevation Model) service, BoundaryAI, Cropland Data Layers, ProfitLayer, ADAPT, etc.

ArcGIS Pro Users can access easily to Ag-Analytics APIs from extended addin toolbar on ArcGIS Pro and also process, manage, analysis about various api service data on ArcGIS Pro platform.

This program will be help for ArcGIS pro users who want to use Ag-Analytics API.

![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/Generalworkflow.png)

## 2. Installation

To use the ArcGIS pro Toolbar function, the following environments must be installed.
> 1) ArcGIS Pro 2.6 install
[ArcGIS Pro 2.6 install](https://pro.arcgis.com/en/pro-app/get-started/install-and-sign-in-to-arcgis-pro.htm)
> 2) .Net framework4.8 
[.Net framework4.8 Download](https://dotnet.microsoft.com/download/)
> 3) Visual studio 2019 install (for developers)
[Visual Studio 2019 Download](https://visualstudio.microsoft.com/downloads/)
> 4) How to add toolbar on arcgis pro
[Intructions](https://awesomeopensource.com/project/Esri/arcgis-pro-sdk-community-samples/) 


## How to add toolbar on arcgispro

   1) please download source code with zip file on git

   2) please save zip file in your favorite directory

   3) please unzip in same directory.

   4) Adding add-in toolbar on ArcGIS Pro.

### Set up add-ins

   1) please run ArcGIS Pro on your pc.

   2) Go to ArcGIS Pro/Project tab/Add-In Manager.

      When you open the Add-In Manager page, ArcGIS Pro searches specified well-known folders for add-in files.
   
      After the files are located, they are installed and appear on the Add-Ins tab of the Add-In Manager.
   
      The Add-Ins tab provides information about available add-ins.


   ![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/add-ins.png)

   3) You can see registered add-ins, and remove add-in if you installed old version.

      You can delete local add-ins from the Add-In Manager.
    

   ![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/del-addin.png) 

      The add-in is marked with a message that it has been deleted. 
      
      It is still visible in the Add-In Manager and is available in the current ArcGIS Pro session.
    
   ![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/del-msg.png)
   
   4) Setting add-in toolbar  in Option tab
      
      ArcGIS Pro searches these folders for add-ins each time you start the application. 
      
      This option is useful if you use a network location to share add-ins within your organization.

   ![Project Image](https://github.com/DavidFullstackdev/ArcGIS-Pro-Addin-Tool/blob/master/images/option.png)
      

   5) Restart ArcGIS Pro.

         noted : If you do not understand this process, please refer to the following link.
   [get-started](https://pro.arcgis.com/en/pro-app/get-started/manage-add-ins.htm)

         Or you can see and refer to our videos.


## 3. Usage

   1) How to login.

   2) How to use HLS service

      The Ag-Analytics® Harmonized Landsat-Sentinel Service (HLS) API provides the service in which a user can provide an area-of-interest (AOI) with additional customized options to 
      
      trieve the dynamics of their land at various times from the Landsat-8 and Sentinel-2 satellites. 

      noted : If you want to know more details , please refer to the following link.
   
   [HLS-Service](https://ag-analytics.portal.azure-api.net/docs/services/harmonized-landsat-sentinel-service/operations/hls-service)

    If you want to know about how to add toolbar on arcgispro , please refer to the following this Video.




   3) How to use DEM service

    The Ag-Analytics® DEM Service API allows for clipping boundaries to the 10 meter USGS DEM map of the United States. 
    
    The service consists of a POST request where the user can pass a GeoJSON boundary, desired output projection as an EPSG code, and a resolution in degrees lat/lon.

    noted : If you want to know more details , please refer to the following link.

   [DEM-Service](https://ag-analytics.portal.azure-api.net/docs/services/dem-service/operations/dem-service)

    If you want to know about how to add toolbar on arcgispro , please refer to the following this Video.

   4) How to use Yield service


    If you want to know about how to add toolbar on arcgispro , please refer to the following this Video.
   
   [Yield service](https://ag-analytics.portal.azure-api.net/docs/services/dem-service/operations/dem-service)

   5) How to use ADAPT service 

   

   6) How to use ProfitLayer service 

    







