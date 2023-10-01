using System;
using System.Drawing;
using AuroraDialogEnhancer.Backend.External;

namespace AuroraDialogEnhancer.Backend.Hooks.Process;

public class WindowInfo
{
    private readonly IntPtr _handle;

    public Rectangle WindowRectangle { get; private set; }

    public Rectangle ClientRectangle { get; private set; }

    public int TitleBarSize { get; private set; }

    public int BorderSize { get; private set; }

    public int BottomYPoint { get; private set; }

    public Rectangle RelativeRightSideOfTheClient { get; set; }

    public Point ClientRectangleRelativePosition { get; private set; }

    public WindowInfo(IntPtr handle, Rectangle clientRectangle, Rectangle windowRectangle)
    {
        _handle = handle;
        SetLocation(clientRectangle, windowRectangle);
    }

    public bool IsMinimized() => NativeMethods.IsIconic(_handle);

    public void SetLocation(Rectangle clientRectangle, Rectangle windowRectangle)
    {
        ClientRectangle = clientRectangle;
        WindowRectangle = windowRectangle;

        BorderSize = (WindowRectangle.Width - WindowRectangle.X - ClientRectangle.Width) / 2;
        TitleBarSize = (WindowRectangle.Height - WindowRectangle.Y - ClientRectangle.Height) - BorderSize;
        ClientRectangleRelativePosition = new Point(WindowRectangle.X + BorderSize, WindowRectangle.Y + TitleBarSize);

        BottomYPoint = ClientRectangleRelativePosition.Y + ClientRectangle.Height - 3;

        RelativeRightSideOfTheClient = new Rectangle(ClientRectangle.Width / 2, 0, ClientRectangle.Width / 2, ClientRectangle.Height);
    }

    public string GetClientSize()
    {
        if (!ClientRectangle.Size.IsEmpty)
        {
            return $"{ClientRectangle.Width}x{ClientRectangle.Height}";
        }

        return IsMinimized() 
            ? Properties.Localization.Resources.WindowInfo_Minimized 
            : $"{ClientRectangle.Width}x{ClientRectangle.Height}";
    }
}
