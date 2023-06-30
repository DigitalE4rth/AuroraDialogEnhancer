using System;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

internal class DialogOptionSearchArea
{
    public int Width { get; }
    public int Height { get; }
    public int WidthOne { get; }
    public int WidthFifty { get; }
    public int BackgroundPadding { get; }
    public OutlineArea OutlineArea { get; }
    
    public DialogOptionSearchArea(int width, int height)
    {
        Width = width;
        Height = height;
        WidthOne = (int) (width * 0.1);
        WidthFifty = (int) (width * 0.5);
        OutlineArea = new OutlineArea( (int) (height * 0.95));
        BackgroundPadding = height - OutlineArea.Height / 2;
    }
}
