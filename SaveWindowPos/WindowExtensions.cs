using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;

namespace SaveWindowPos;

/// <summary>
/// Extension methods for saving and restoring the position of a WPF Window using JSON strings.
/// </summary>
public static class WindowExtensions
{
  /// <summary>
  /// Gets the window position as a JSON string.
  /// </summary>
  /// <param name="w">The Window</param>
  /// <returns>The window position as a JSON string.</returns>
    public static string GetWindowPositionString(this Window w)
    {
        var wp = GetWinPos(w);
        return JsonSerializer.Serialize(wp, new JsonSerializerOptions { IncludeFields = true });
    }

    /// <summary>
    /// Restores the window position from a JSON string.
    /// </summary>
    /// <param name="w">The Window</param>
    /// <param name="wpStr">JSON string</param>
    public static void RestoreWindowPosition(this Window w, string wpStr)
    {
        var wpCurrent = GetWinPos(w);
        var wpNew = GetWinPosString(wpStr);

        wpNew.length = Marshal.SizeOf(typeof(WinPos));
        wpNew.flags = 0;
        wpNew.showCmd = (wpNew.showCmd == SwShowminimized ? SwShownormal : wpNew.showCmd);
        wpNew.minPosition = wpCurrent.minPosition;
        wpNew.maxPosition = wpCurrent.maxPosition;

        try
        {
            // Load window placement details for previous application session from application settings
            // Note - if window was closed on a monitor that is now disconnected from the computer,
            //        SetWindowPlacement will place the window onto a visible monitor.
            var hwnd = new WindowInteropHelper(w).Handle;
            SetWindowPlacement(hwnd, ref wpNew);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Deserializes a JSON string into a WinPos struct.
    /// </summary>
    /// <param name="wpStr">JSON string</param>
    /// <returns>A WinPos struct.</returns>
    private static WinPos GetWinPosString(string wpStr)
    {
        if (string.IsNullOrWhiteSpace(wpStr))
        {
            return new WinPos();
        }

        try
        {
            return JsonSerializer.Deserialize<WinPos>(wpStr, new JsonSerializerOptions { IncludeFields = true });
        }
        catch (Exception)
        {
            return new WinPos();
        }
    }

    /// <summary>
    /// Gets the window position as a WinPos struct.
    /// </summary>
    /// <param name="w">The Window</param>
    /// <returns>A WinPos struct.</returns>
    private static WinPos GetWinPos(Window w)
    {
        // Persist window placement details to application settings
        var hwnd = new WindowInteropHelper(w).Handle;
        GetWindowPlacement(hwnd, out var wp);
        return wp;
    }

    /// <summary>
    /// Ensure a window is visible on the screen.
    /// If it is partially hidden, snap it on screen.
    /// </summary>
    /// <param name="w">The Window</param>
    public static void EnsureIsVisible(this Window w)
    {
        double winLeft = w.Left;
        double winRight = w.Left + w.Width;
        double winTop = w.Top;
        double winBottom = w.Top + w.Height;

        double screenLeft = SystemParameters.VirtualScreenLeft;
        double screenRight = SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth;
        double screenTop = SystemParameters.VirtualScreenTop;
        double screenBottom = SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight;

        if (winRight > screenRight)
            w.Left = screenRight - w.Width;

        if (winBottom > screenBottom)
            w.Top = screenBottom - w.Height;

        if (winLeft < screenLeft)
            w.Left = screenLeft;

        if (winTop < screenTop)
            w.Top = screenTop;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect(int left, int top, int right, int bottom)
    {
        public int Left = left;
        public int Top = top;
        public int Right = right;
        public int Bottom = bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WinPos
    {
        public int length = 0;
        public int flags = 0;
        public int showCmd = 1;
        public Point minPosition = new(-32000, -32000);
        public Point maxPosition = new(-1, -1);
        public Rect normalPosition = new(100, 100, 700, 900);

        public WinPos() { }
    }

    #region Win32 API declarations to set and get window placement

    [DllImport("user32.dll")]
    private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WinPos lpwndpl);

    [DllImport("user32.dll")]
    private static extern bool GetWindowPlacement(IntPtr hWnd, out WinPos lpwndpl);

    private const int SwShownormal = 1;
    private const int SwShowminimized = 2;

    #endregion
}