# GodotSharpInstaller

I was bored with manually checking if an update for Godot.NET 4 was available, so I created this little program that automatically checks for updates

Additionally, it can "install" and "uninstall" each version of Godot.NET (if installed from GodotSharpInstaller)

Of course, it is possible to install several versions (Godot 4.2.1 and Godot 3.5.3, for example)

## Use

The program is accessible from the system tray. By double-clicking the system tray icon you can access the main form app and "install" or "uninstall" any version you wish

In the main form app :

 - To minimize the program, use the cross in the top right corner
 - If you want the program to start with window don't forget to check the "Start with Window" in the "options" menu
 - If you want the app to do not check the Godot 4 versions don't forget to check it too in the "options" menu
 - You can close GodotSharpInstaller with the "Exit" in the "options" menu

In the system tray icon :

 - You can open the main form app
 - You can manually check for updates
 - You can close GodotSharpInstaller

 This app doesn't update automatically, you need to open the main form app and click on the install button to do so. Additionally you have to "uninstall" the previous version yourself, by selecting the version you had previously and clicking on the uninstall button
 
## Technical information

The program is "installing" versions of Godot in ```C:/Users/%YourUserName%/AppData/Local/GodotSharpApp```  
The AppData folder is hiden by default see [this link](https://support.microsoft.com/en-us/windows/view-hidden-files-and-folders-in-windows-97fbc472-c603-9d90-91d0-1166d1d9f4b5#WindowsVersion=Windows_11)

If the "Start with Windows" option is activated, a registry key is created at ```HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run```

The program run in x64 and was only tested on Win10 and Win11

## Disclaimer

I am in no way responsible for the way you're going to use this program
