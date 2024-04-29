# GodotSharpInstaller

I was bored to check manually if a Godot.NET 4 update was available, so I created this little soft that automaticcaly checks for updates

Additionally, it can "install" and "uninstall" every Godot.NET version (if it was installed from GodotSharpInstaller)

Of course, it is possible to install several versions (Godot 4.2.1 and Godot 3.5.3 for example)

## Use

The app is accessible in the system tray. By double click the system tray icon you can access the main form app and "install" or "uninstall" all the versions you want

In the main form app :

 - To reduce close the main form app, use the cross in the top right corner
 - If you want the app to be started with window don't forget to check it in the "options" menu
 - If you want the app to do not check the Godot 4 versions don't forget to check it too in the "options" menu
 - You can close GodotSharpInstaller with the "Exit" int the "options" menu

In the system tray icon :

 - You can open the main form app
 - You can manually check for an update
 - You can close GodotSharpInstaller

 This app doesn't make an update automatically, you have to open the main form app and click on the install button to do it. Additionally you have to "uninstall" the previous version yourself, by selecting the version you have before and click on the uninstall button
 
## Technical information

The app is "installing" versions of Godot in ```C:/Users/%YourUserName%/AppData/Local/GodotSharpApp```
  The AppData folder is hiden by default see [this link](https://support.microsoft.com/en-us/windows/view-hidden-files-and-folders-in-windows-97fbc472-c603-9d90-91d0-1166d1d9f4b5#WindowsVersion=Windows_11)

If the "Start with Windows" option is activated, a registry key is created at ```HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run```
