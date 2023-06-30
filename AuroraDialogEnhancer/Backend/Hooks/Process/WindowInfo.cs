using System;
using System.Drawing;
using AuroraDialogEnhancer.Backend.External;

namespace AuroraDialogEnhancer.Backend.Hooks.Process;

public class WindowInfo
{
    public string ClientSize { get; private set; }

    public WindowInfo(IntPtr handle, Rectangle clientRectangle, Rectangle windowRectangle)
    {
        Handle = handle;
        SetLocation(clientRectangle, windowRectangle);
        ClientSize = string.Empty;
    }

    public void SetLocation(Rectangle clientRectangle, Rectangle windowRectangle)
    {
        ClientRectangle = clientRectangle;
        WindowRectangle = windowRectangle;

        BorderSize = (WindowRectangle.Width - WindowRectangle.X - ClientRectangle.Width) / 2;
        TitleBarSize = (WindowRectangle.Height - WindowRectangle.Y - ClientRectangle.Height) - BorderSize;
        ClientRectangleRelativePosition = new Point(WindowRectangle.X + BorderSize, WindowRectangle.Y + TitleBarSize);

        BottomRightAbsolutePoint = new Point(ClientRectangleRelativePosition.X + ClientRectangle.Width - 5,
                                             ClientRectangleRelativePosition.Y + ClientRectangle.Height - 5);

        RelativeRightSideOfTheClient = new Rectangle(ClientRectangle.Width / 2, 0, ClientRectangle.Width / 2, ClientRectangle.Height);
    }

    private IntPtr Handle { get; }

    /// <summary>
    /// Target window client rectangle.
    /// </summary>
    /// <remarks>
    /// Actual window size without borders and title bar.
    /// </remarks>
    public Rectangle ClientRectangle { get; private set; }

    public Point BottomRightAbsolutePoint { get; private set; }

    /// <summary>
    /// Target window rectangle.
    /// </summary>
    /// <remarks>
    /// Relative size to the screen X=0, Y=0 coordinates, with window's title bar and borders.
    /// </remarks>
    public Rectangle WindowRectangle { get; private set; }

    /// <summary>
    /// The size of one window border.
    /// </summary>
    public int BorderSize { get; private set; }

    /// <summary>
    /// The size of the window title bar.
    /// </summary>
    public int TitleBarSize { get; private set; }

    public bool IsMinimized => NativeMethods.IsIconic(Handle);

    public Rectangle RelativeRightSideOfTheClient { get; set; }

    public Point ClientRectangleRelativePosition { get; private set; }

    public void SetClientSize()
    {
        if (IsMinimized)
        {
            ClientSize = Properties.Localization.Resources.WindowInfo_Minimized;
            return;
        }

        ClientSize = ClientRectangle.Width + "x" + ClientRectangle.Height;
    }
}
