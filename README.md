Extension methods for saving and restoring the position of a WPF Window using JSON strings.

## Methods

```csharp
public static string GetWindowPositionString(Window w)
```
Gets the window position as a JSON string.
#### Parameters
[Window](https://learn.microsoft.com/en-us/dotnet/api/system.windows.window?view=windowsdesktop-8.0) `w` - The Window
#### Returns
[String](https://docs.microsoft.com/en-us/dotnet/api/system.string) The window position as a JSON string.

```csharp
public static void RestoreWindowPosition(Window w, string wpStr)
```
Restores the window position from a JSON string.
#### Parameters
[Window](https://learn.microsoft.com/en-us/dotnet/api/system.windows.window?view=windowsdesktop-8.0) `w` - The Window
`wpStr` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string) JSON string


```csharp
public static void EnsureIsVisible(Window w)
```
Ensure a window is visible on the screen. If it is partially hidden, snap it on screen.
#### Parameters
[Window](https://learn.microsoft.com/en-us/dotnet/api/system.windows.window?view=windowsdesktop-8.0) `w` - The Window
