using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplateFactory
{
    private readonly Dictionary<double, Type> _dynamicTemplates = new()
    {
        { 1.77, typeof(DynamicTemplate1P77) },
        { 2.30, typeof(DynamicTemplate2P30) },
        { 2.33, typeof(DynamicTemplate2P33) },
        { 2.35, typeof(DynamicTemplate2P35) },
        { 2.37, typeof(DynamicTemplate2P37) },
        { 2.38, typeof(DynamicTemplate2P38) },
        { 2.4,  typeof(DynamicTemplate2P4)  },
        { 3.5,  typeof(DynamicTemplate3P5)  },
    };

    public DynamicTemplateBase GetTemplate(Size size)
    {
        var ratio = (double) size.Width / size.Height;
        var closestValue = _dynamicTemplates.OrderBy(pair => Math.Abs(pair.Key - ratio)).First();
        //Debug.WriteLine($"Ratio: {ratio}, Closest ratio: {closestValue}");
        return (DynamicTemplateBase) Activator.CreateInstance(closestValue.Value);
    }
}
