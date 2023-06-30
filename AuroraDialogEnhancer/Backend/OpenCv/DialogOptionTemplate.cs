using System;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace AuroraDialogEnhancer.Backend.OpenCv;

public class DialogOptionTemplate : IDisposable
{
    public Mat? TemplateMat { get; set; }

    public Mat? MaskMat { get; set; }

    public DialogOptionTemplate(Bitmap template, Bitmap mask)
    {
        TemplateMat = template.ToMat().CvtColor(ColorConversionCodes.BGR2GRAY);
        MaskMat = mask.ToMat().CvtColor(ColorConversionCodes.BGR2GRAY);

        template.Dispose();
        mask.Dispose();
    }

    public void Dispose()
    {
        TemplateMat?.Dispose();
        MaskMat?.Dispose();
    }
}
