   M S B A M L   `   `   `  ����`  \ArcGIS.Desktop.Framework, Version=12.6.0.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad86wQclr-namespace:ArcGIS.Desktop.Framework.Controls;assembly=ArcGIS.Desktop.Framework!ArcGIS.Desktop.Framework.Controls  5   	   a ]ArcGIS.Desktop.Extensions, Version=12.6.0.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad86hJclr-namespace:ArcGIS.Desktop.Extensions;assembly=ArcGIS.Desktop.ExtensionsArcGIS.Desktop.Extensions 5   	    Ag_Analytics_Toolbar@   :Ag_Analytics_Toolbar.Azure_Active_Directory_B2C.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   dcontrolsQclr-namespace:ArcGIS.Desktop.Framework.Controls;assembly=ArcGIS.Desktop.Framework    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   a
extensionsJclr-namespace:ArcGIS.Desktop.Extensions;assembly=ArcGIS.Desktop.Extensions     5   	     � Title$  
MainWindow��5	   	   $	��300��6   $	��300��6)    � WindowStartupLocation$ CenterOwner=�5
   	    � 	Resources 5       �� MergedDictionaries<  6ArcGIS.Desktop.Extensions.DesignOnlyResourceDictionary   Source�� 5   
    5        5      $S Mpack://application:,,,/ArcGIS.Desktop.Framework;component\Themes\Default.xaml?�6:   %    5      .��5      � 
������� � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��20 �6   �� 5      $��* �6   �� 5      $��20 �6   �� 5      $��* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   .+�5      �� -   5         ��Name$ SignInButton��5      $��Sign In��6+   $��Right=�6=   $��5q�6Y   $��5q�6�   $��Segoe Ui)�6�   �� 5      -   5        $ CallApiButton��5      $��Call API��6,   $��Right=�6?   $��5q�6[   $��5q�6�   $y�	Collapsed=�6�   $��Segoe Ui)�6�   �� 5      -   5        $ EditProfileButton��5      $��Edit Profile��60   $��Right=�6G   $��5q�6c   $��5q�6�   $y�	Collapsed=�6�   $��Segoe Ui)�6�   �� 5      -   5        $ SignOutButton��5      $��Sign Out��6,   $��Right=�6?   $��5q�6[   $��5q�6�   $y�	Collapsed=�6�   $��Segoe Ui)�6�   5       �� 5"   
   $��1��6   $��API Call Results��6   $��0,0,0,-5q�68   $��Segoe Ui)�6J   �� 5#   
   -   5         ��Name$ 
ResultText��5#      $��2��6   	 �� TextWrapping$
	 Wrap=�63   $��5q�6G   $��Segoe Ui)�6R   $
��Auto=�6h   �� 5$   
   $��3��6   $��
Token Info��6   $��0,0,0,-5q�62   $��Segoe Ui)�6D   �� 5%   
   -   5        $ TokenInfoText��5%      $��4��6   $
	 Wrap=�66   $��5q�6J   $��Segoe Ui)�6U   $
��Auto=�6k   5&      5'      