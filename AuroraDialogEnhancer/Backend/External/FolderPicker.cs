using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Interop;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace AuroraDialogEnhancer.Backend.External;

public class FolderPicker
{
    public virtual string? ResultPath { get; protected set; }
    public virtual string? ResultName { get; protected set; }
    public virtual string? InputPath { get; set; }
    public virtual bool ForceFileSystem { get; set; }
    public virtual string? Title { get; set; }
    public virtual string? OkButtonLabel { get; set; }
    public virtual string? FileNameLabel { get; set; }

    protected virtual int SetOptions(int options)
    {
        if (ForceFileSystem)
        {
            options |= (int) FOS.FOS_FORCEFILESYSTEM;
        }
        return options;
    }

    // For WPF support
    public bool? ShowDialog(Window? owner = null, bool throwOnError = false)
    {
        owner ??= Application.Current.MainWindow;
        return ShowDialog(owner is not null ? new WindowInteropHelper(owner).Handle : IntPtr.Zero, throwOnError);
    }

    // For all .NET
    public virtual bool? ShowDialog(IntPtr owner, bool throwOnError = false)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        var dialog = (IFileOpenDialog)new FileOpenDialog();
        if (InputPath is not null && !string.IsNullOrEmpty(InputPath))
        {
            if (CheckHr(SHCreateItemFromParsingName(InputPath, null, typeof(IShellItem).GUID, out var item), throwOnError) != 0)
            {
                return null;
            }

            dialog.SetFolder(item);
        }

        var options = FOS.FOS_PICKFOLDERS;
        options = (FOS)SetOptions((int)options);
        dialog.SetOptions(options);

        if (Title is not null)
        {
            dialog.SetTitle(Title);
        }

        if (OkButtonLabel is not null)
        {
            dialog.SetOkButtonLabel(OkButtonLabel);
        }

        if (FileNameLabel is not null)
        {
            dialog.SetFileName(FileNameLabel);
        }

        if (owner == IntPtr.Zero)
        {
            owner = Process.GetCurrentProcess().MainWindowHandle;
            if (owner == IntPtr.Zero)
            {
                owner = GetDesktopWindow();
            }
        }

        var hr = dialog.Show(owner);
        if (hr == ERRORCANCELLED)
        {
            return null;
        }

        if (CheckHr(hr, throwOnError) != 0)
        {
            return null;
        }

        if (CheckHr(dialog.GetResult(out var result), throwOnError) != 0)
        {
            return null;
        }

        if (CheckHr(result.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out var path), throwOnError) != 0)
        {
            return null;
        }

        ResultPath = path;

        if (CheckHr(result.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEEDITING, out path), false) == 0)
        {
            ResultName = path;
        }
        return true;
    }

    private static int CheckHr(int hr, bool throwOnError)
    {
        if (hr == 0)
        {
            return hr;
        }

        if (throwOnError)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        return hr;
    }

    [DllImport("shell32")]
    private static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx? pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItem ppv);

    [DllImport("user32")]
    private static extern IntPtr GetDesktopWindow();

    private const int ERRORCANCELLED = unchecked((int)0x800704C7);

    // CLSID_FileOpenDialog
    [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")] 
    private class FileOpenDialog
    {
    }

    [ComImport, Guid("42f85136-db7e-439c-85f1-e4075d135fc8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileOpenDialog
    {
        // IModalWindow
        [PreserveSig] int Show(IntPtr parent);
        // Not fully defined
        [PreserveSig] int SetFileTypes(); 
        [PreserveSig] int SetFileTypeIndex(int iFileType);
        [PreserveSig] int GetFileTypeIndex(out int piFileType);
        // Not fully defined
        [PreserveSig] int Advise();
        [PreserveSig] int Unadvise();
        [PreserveSig] int SetOptions(FOS fos);
        [PreserveSig] int GetOptions(out FOS pfos);
        [PreserveSig] int SetDefaultFolder(IShellItem psi);
        [PreserveSig] int SetFolder(IShellItem psi);
        [PreserveSig] int GetFolder(out IShellItem ppsi);
        [PreserveSig] int GetCurrentSelection(out IShellItem ppsi);
        [PreserveSig] int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        [PreserveSig] int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
        [PreserveSig] int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
        [PreserveSig] int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
        [PreserveSig] int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
        [PreserveSig] int GetResult(out IShellItem ppsi);
        [PreserveSig] int AddPlace(IShellItem psi, int alignment);
        [PreserveSig] int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
        [PreserveSig] int Close(int hr);
        // Not fully defined
        [PreserveSig] int SetClientGuid();
        [PreserveSig] int ClearClientData();
        [PreserveSig] int SetFilter([MarshalAs(UnmanagedType.IUnknown)] object pFilter);
        [PreserveSig] int GetResults([MarshalAs(UnmanagedType.IUnknown)] out object ppenum);
        [PreserveSig] int GetSelectedItems([MarshalAs(UnmanagedType.IUnknown)] out object ppsai);
    }

    [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItem
    {
        // Not fully defined
        [PreserveSig] int BindToHandler();
        // Not fully defined
        [PreserveSig] int GetParent();
        [PreserveSig] int GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
        // Not fully defined
        [PreserveSig] int GetAttributes();
        // Not fully defined
        [PreserveSig] int Compare();
    }

    private enum SIGDN : uint
    {
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
        SIGDN_FILESYSPATH = 0x80058000,
        SIGDN_NORMALDISPLAY = 0,
        SIGDN_PARENTRELATIVE = 0x80080001,
        SIGDN_PARENTRELATIVEEDITING = 0x80031001,
        SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
        SIGDN_PARENTRELATIVEPARSING = 0x80018001,
        SIGDN_URL = 0x80068000
    }

    [Flags]
    private enum FOS
    {
        FOS_OVERWRITEPROMPT = 0x2,
        FOS_STRICTFILETYPES = 0x4,
        FOS_NOCHANGEDIR = 0x8,
        FOS_PICKFOLDERS = 0x20,
        FOS_FORCEFILESYSTEM = 0x40,
        FOS_ALLNONSTORAGEITEMS = 0x80,
        FOS_NOVALIDATE = 0x100,
        FOS_ALLOWMULTISELECT = 0x200,
        FOS_PATHMUSTEXIST = 0x800,
        FOS_FILEMUSTEXIST = 0x1000,
        FOS_CREATEPROMPT = 0x2000,
        FOS_SHAREAWARE = 0x4000,
        FOS_NOREADONLYRETURN = 0x8000,
        FOS_NOTESTFILECREATE = 0x10000,
        FOS_HIDEMRUPLACES = 0x20000,
        FOS_HIDEPINNEDPLACES = 0x40000,
        FOS_NODEREFERENCELINKS = 0x100000,
        FOS_OKBUTTONNEEDSINTERACTION = 0x200000,
        FOS_DONTADDTORECENT = 0x2000000,
        FOS_FORCESHOWHIDDEN = 0x10000000,
        FOS_DEFAULTNOMINIMODE = 0x20000000,
        FOS_FORCEPREVIEWPANEON = 0x40000000,
        FOS_SUPPORTSTREAMABLEITEMS = unchecked((int)0x80000000)
    }
}
