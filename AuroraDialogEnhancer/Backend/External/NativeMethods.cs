﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;

namespace AuroraDialogEnhancer.Backend.External;

public class NativeMethods
{
    /// <summary>
    /// The callback function is not mapped into the address space of the process that generates the event. Because the hook function is called across process boundaries, the system must queue events. Although this method is asynchronous, events are guaranteed to be in sequential order. For more information, see <see href="https://learn.microsoft.com/en-us/windows/desktop/WinAuto/out-of-context-hook-functions">Out-of-Context Hook Functions</see>.
    /// </summary>
    public const uint WINEVENT_OUTOFCONTEXT = 0x0000;

    /// <summary>
    /// The foreground window has changed. The system sends this event even if the foreground window has changed to another window in the same thread. Server applications never send this event.
    /// </summary>
    public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;

    /// <summary>
    /// A window has received mouse capture. This event is sent by the system, never by servers.
    /// </summary>
    public const uint EVENT_SYSTEM_CAPTURESTART = 0x0008;

    /// <summary>
    /// The movement or resizing of a window has finished. This event is sent by the system, never by servers.
    /// </summary>
    public const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

    /// <summary>
    /// A window object is about to be restored. This event is sent by the system, never by servers.
    /// </summary>
    public const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;

    /// <summary>
    /// The system ID of the flag to get the setting whether the primary mouse button has been swapped.
    /// </summary>
    public const int SM_SWAPBUTTON = 23;

    public static IntPtr User32Pointer { get; } = LoadLibrary("user32.dll");

    /// <summary>
    /// Loads the library.
    /// </summary>
    /// <param name="lpFileName">Name of the library</param>
    /// <returns>A handle to the library</returns>
    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string lpFileName);

    /// <summary>
    /// Retrieves the dimensions of the bounding rectangle of the specified window.
    /// </summary>
    /// <remarks>
    /// The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.
    /// </remarks>
    /// <param name="handle">A handle to the window.</param>
    /// <param name="rect">A pointer to a <see cref="Rectangle"/> structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</param>
    /// <returns>If the function succeeds, the return value is <see langword="true"/>; otherwise <see langword="false"/>.</returns>
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr handle, ref Rectangle rect);

    /// <summary>
    /// Retrieves the coordinates of a window's client area.
    /// </summary>
    /// <remarks>
    /// The client coordinates specify the upper-left and lower-right corners of the client area. Because client coordinates are relative to the upper-left corner of a window's client area, the coordinates of the upper-left corner are (0,0).
    /// </remarks>
    /// <param name="hWnd">A handle to the window whose client coordinates are to be retrieved.</param>
    /// <param name="lpRect">A pointer to a <see cref="Rectangle"/> structure that receives the client coordinates. The left and top members are zero. The right and bottom members contain the width and height of the window.</param>
    /// <returns>If the function succeeds, the return value is <see langword="true"/>; otherwise <see langword="false"/>.</returns>
    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

    /// <summary>
    /// Sets the keyboard focus to the specified window. The window must be attached to the calling thread's message queue.
    /// </summary>
    /// <param name="hWnd">A handle to the window that will receive the keyboard input. If this parameter is NULL, keystrokes are ignored.</param>
    /// <returns>
    /// If the function succeeds, the return value is the handle to the window that previously had the keyboard focus. If the hWnd parameter is invalid or the window is not attached to the calling thread's message queue, the return value is NULL. To get extended error information, call <see href="https://learn.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</see> function.<br/>
    /// Extended error ERROR_INVALID_PARAMETER (0x57) means that window is in disabled state.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    /// <summary>
    /// Sets an event hook function for a range of events.
    /// </summary>
    /// <param name="eventMin">Specifies the event constant for the lowest event value in the range of events that are handled by the hook function. This parameter can be set to EVENT_MIN to indicate the lowest possible event value.</param>
    /// <param name="eventMax">Specifies the event constant for the highest event value in the range of events that are handled by the hook function. This parameter can be set to EVENT_MAX to indicate the highest possible event value.</param>
    /// <param name="hmodWinEventProc">Handle to the DLL that contains the hook function at lpfnWinEventProc, if the WINEVENT_INCONTEXT flag is specified in the dwFlags parameter. If the hook function is not located in a DLL, or if the WINEVENT_OUTOFCONTEXT flag is specified, this parameter is NULL.</param>
    /// <param name="lpfnWinEventProc">Pointer to the event hook function. For more information about this function, see WinEventProc.</param>
    /// <param name="idProcess">Specifies the ID of the process from which the hook function receives events. Specify zero (0) to receive events from all processes on the current desktop.</param>
    /// <param name="idThread">Specifies the ID of the thread from which the hook function receives events. If this parameter is zero, the hook function is associated with all existing threads on the current desktop.</param>
    /// <param name="dwFlags">Flag values that specify the location of the hook function and of the events to be skipped.</param>
    /// <returns>If successful, returns an <see cref="IntPtr"/> value that identifies this event hook instance. If unsuccessful, returns <see cref="IntPtr.Zero"/>.</returns>
    [DllImport("user32.dll")]
    public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    /// <summary>
    /// An application-defined callback (or hook) function that the system calls in response to events generated by an accessible object. The hook function processes the event notifications as required. Clients install the hook function and request specific types of event notifications by calling <see cref="SetWinEventHook"/>.
    /// </summary>
    /// <param name="hWinEventHook">Handle to an event hook function. This value is returned by <see cref="SetWinEventHook"/> when the hook function is installed and is specific to each instance of the hook function.</param>
    /// <param name="eventType">Specifies the event that occurred. This value is one of the <see href="https://learn.microsoft.com/en-us/windows/desktop/WinAuto/event-constants">event-constants</see>.</param>
    /// <param name="hwnd">Handle to the window that generates the event, or <see langword="null"/> if no window is associated with the event. For example, the mouse pointer is not associated with a window.</param>
    /// <param name="idObject">Identifies the object associated with the event. This is one of the <see href="https://learn.microsoft.com/en-us/windows/desktop/WinAuto/object-identifiers">object identifiers</see> or a custom object ID.</param>
    /// <param name="idChild">Identifies whether the event was triggered by an object or a child element of the object. If this value is CHILDID_SELF, the event was triggered by the object; otherwise, this value is the child ID of the element that triggered the event.</param>
    /// <param name="dwEventThread">The identifier event thread.</param>
    /// <param name="dwmsEventTime">Specifies the time, in milliseconds, that the event was generated.</param>
    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    /// <summary>
    /// Removes an event hook function created by a previous call to <see cref="SetWinEventHook"/>.
    /// </summary>
    /// <param name="hWinEventHook">Handle to the event hook returned in the previous call to <see cref="SetWinEventHook"/>.</param>
    /// <returns>If the function succeeds, the return value is <see langword="true"/>; otherwise <see langword="false"/>.</returns>
    [DllImport("user32.dll")]
    public static extern int UnhookWinEvent(IntPtr hWinEventHook);

    /// <summary>
    /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="lpdwProcessId">A pointer to a variable that receives the process identifier. If this parameter is not NULL, GetWindowThreadProcessId copies the identifier of the process to the variable; otherwise, it does not. If the function fails, the value of the variable is unchanged.</param>
    /// <returns>If the function succeeds, the return value is the identifier of the thread that created the window. If the window handle is invalid, the return value is zero. To get extended error information, call GetLastError.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    /// <summary>
    /// Retrieves a handle to the foreground window (the window with which the user is currently working).
    /// </summary>
    /// <remarks>
    /// The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
    /// </remarks>
    /// <returns>The return value is a handle to the foreground window. The foreground window can be <see langword="null"/> in certain circumstances, such as when a window is losing activation.</returns>
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);
    public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Must initialize CursorInfoStruct byte size.
    /// </summary>
    [DllImport("user32.dll")]
    public static extern bool GetCursorInfo(ref CursorInfoStruct pci);

    [DllImport("user32.dll")]
    public static extern bool IsIconic(IntPtr handle);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int bSwap);

    public static bool IsMousePrimaryButtonSwapped => GetSystemMetrics(SM_SWAPBUTTON) != 0;

    [DllImport("kernel32.dll", SetLastError = true)]  
    public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);

    [Flags]
    public enum ProcessAccessFlags : uint
    {
        QueryLimitedInformation = 0x00001000
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr hObject);
}
