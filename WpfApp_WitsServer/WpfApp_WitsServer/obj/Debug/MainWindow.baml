   M S B A M L   `   `   `  ����L  HWpfApp_WitsServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:WpfApp_WitsServerWpfApp_WitsServer  5   	   T PMahApps.Metro, Version=2.0.0.0, Culture=neutral, PublicKeyToken=51482d6f650b2b3fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls 5	   	   "    WpfApp_WitsServer.MainWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:WpfApp_WitsServer     5   	   Kmah;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5	   	     � Title$  井场数据WITS模拟器��5   	   $	��450��6   $	��800��6*   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6    � ColumnDefinitions 5   
   �� 5      6!   �� 5      6!   .+�5   
   ]� $��0��6   $��2��6$   .+�5      c� $	��Top=�6   
�������.@�5      �� -   5        $��Center=�5      $��5q�66   ��5      �� 5      .+�5      �� $
��Fill=�6$   $V��PE:\xt18\6.Repos\GitHub\Bay\WpfApp_WitsServer\WpfApp_WitsServer\Images\listen.PNG��63   $��38��6�   $��47��6�   6�   �� 5      $��Center=�6(   $��
TimeNewMan)�6E   $��Oblique$�6]   $
��Bold"�6q   
����+��Listen6�   5      5      �� 5      -   5        $��Center=�5      $��5q�66   ��5       �� 5!      .+�5"      �� $
��Fill=�6$   $T��NE:\xt18\6.Repos\GitHub\Bay\WpfApp_WitsServer\WpfApp_WitsServer\Images\send.PNG��63   $��38��6�   $��47��6�   6�   �� 5#      $��Center=�6(   $��
TimeNewMan)�6E   $��Oblique$�6]   $
��Bold"�6q   
����+��Send6�   5$      5&      �� 5'      $��Center=�6   $��5q�66   ��5(      �� 5)      .+�5*      �� $
��Fill=�6$   $T��NE:\xt18\6.Repos\GitHub\Bay\WpfApp_WitsServer\WpfApp_WitsServer\Images\exit.PNG��63   $��38��6�   $��47��6�   6�   �� 5+      $��Center=�6(   $��
TimeNewMan)�6E   $��Oblique$�6]   $
��Bold"�6q   
����+��Exit6�   5,      5.      50      51      �� 52   
   $��Config��6   $��1��6#   .��53      �  54      �� 55      $
��auto �6$   64   �� 56      6&   .+�5@      �� $��5q�6   .@�5A      �� $��Chart 7��6   $��3q�6/   $��20(�6:   .��5B      &   System.Windows.Controls.DataGrid  -   5          Name$ Wits_DataGrid��5B   #   5C      �� 5D      $��Chart 8��6   $��3q�6/   5F      �� 5G      $��Chart 1��6   $��3q�6/   5I      5J      5K      5M      �� 5N   
   $��Result��6   $��1��6#   $��1��63   .��5O      �� -   5         ��Name$ ResBox��5O      6(   5P      5Q      5R      