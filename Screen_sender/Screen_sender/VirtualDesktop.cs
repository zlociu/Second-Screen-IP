using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public class VirtualDesktop : IDisposable
{
    // These security descriptors below are required to
    // let us manipulate the desktop objects.
    internal enum DESKTOP_ACCESS_MASK : uint
    {
        DESKTOP_NONE = 0,
        DESKTOP_READOBJECTS = 0x0001,
        DESKTOP_CREATEWINDOW = 0x0002,
        DESKTOP_CREATEMENU = 0x0004,
        DESKTOP_HOOKCONTROL = 0x0008,
        DESKTOP_JOURNALRECORD = 0x0010,
        DESKTOP_JOURNALPLAYBACK = 0x0020,
        DESKTOP_ENUMERATE = 0x0040,
        DESKTOP_WRITEOBJECTS = 0x0080,
        DESKTOP_SWITCHDESKTOP = 0x0100,

        EVERYTHING = (DESKTOP_READOBJECTS | DESKTOP_CREATEWINDOW | DESKTOP_CREATEMENU |
                  DESKTOP_HOOKCONTROL | DESKTOP_JOURNALRECORD | DESKTOP_JOURNALPLAYBACK |
                  DESKTOP_ENUMERATE | DESKTOP_WRITEOBJECTS | DESKTOP_SWITCHDESKTOP),
    }

    #region Variables
    public IntPtr DesktopPtr;     // This will point to the current desktop we are using.
    public string _sMyDesk;       // This will hold the name for the desktop object we created.
    IntPtr _hOrigDesktop;         // This will remember the very first desktop we spawned on.
    #endregion

    #region DLL Definitions
    [DllImport("user32.dll", EntryPoint = "CloseDesktop", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseDesktop(IntPtr handle);

    [DllImport("user32.dll")]
    private static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode,
                           int dwFlags, long dwDesiredAccess, IntPtr lpsa);

    [DllImport("kernel32.dll")]
    public static extern int GetCurrentThreadId();

    [DllImport("user32.dll")]
    public static extern IntPtr GetThreadDesktop(int dwThreadId);

    [DllImport("user32.dll")]
    public static extern bool SetThreadDesktop(IntPtr hDesktop);

    [DllImport("user32.dll")]
    private static extern bool SwitchDesktop(IntPtr hDesktop);
    #endregion

    #region Disposal Methods
    // Switch to the desktop we were on before.
    public void Dispose()
    {
        SwitchToOriginal();
        ((IDisposable)this).Dispose();
    }

    // Delete our custom one.
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CloseDesktop(DesktopPtr);
        }
    }

    // ... flush!
    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Methods
    public IntPtr GetCurrentDesktopPtr()
    {
        return GetThreadDesktop(GetCurrentThreadId());
    }

    private IntPtr LaunchDesktop()
    {
        return CreateDesktop(_sMyDesk, IntPtr.Zero, IntPtr.Zero,
                     0, (long)DESKTOP_ACCESS_MASK.EVERYTHING, IntPtr.Zero);
    }

    public void ShowDesktop()
    {
        SetThreadDesktop(DesktopPtr);
        SwitchDesktop(DesktopPtr);
    }

    public void SwitchToOriginal()
    {
        SwitchDesktop(_hOrigDesktop);
        SetThreadDesktop(_hOrigDesktop);
    }
    #endregion

    #region Constructors
    public VirtualDesktop()
    {
        _sMyDesk = "";
    }

    public VirtualDesktop(string sDesktopName)
    {
        _hOrigDesktop = GetCurrentDesktopPtr();
        _sMyDesk = sDesktopName;
        DesktopPtr = LaunchDesktop();
    }
    #endregion

}
