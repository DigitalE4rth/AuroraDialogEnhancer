using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplateFactory
{
    private readonly Dictionary<double, Type> _dynamicTemplates = new()
    {
        //{ 1.25,   typeof(DynamicTemplate1P25)   },
        //{ 1.33,   typeof(DynamicTemplate1P33)   },
        //{ 1.5,    typeof(DynamicTemplate1P5)    },
        //{ 1.6,    typeof(DynamicTemplate1P60)   },
        //{ 1.66,   typeof(DynamicTemplate1P66)   },
        { 1.77,   typeof(DynamicTemplate1P77)   },
        //{ 2.30,   typeof(DynamicTemplate2P30)   },
        //{ 2.33,   typeof(DynamicTemplate2P33)   },
        //{ 2.35,   typeof(DynamicTemplate2P35)   },
        //{ 2.37,   typeof(DynamicTemplate2P37)   },
        //{ 2.3880, typeof(DynamicTemplate2P3880) },
        //{ 2.3888, typeof(DynamicTemplate2P3888) },
        //{ 2.4,    typeof(DynamicTemplate2P4)    },
        //{ 3.2,    typeof(DynamicTemplate3P2)    },
        //{ 3.55,   typeof(DynamicTemplate3P55)   }
    };

    public DynamicTemplateBase GetTemplate(Size size)
    {
        var ratio = (double) size.Width / size.Height;
        var closestValue = _dynamicTemplates.OrderBy(pair => Math.Abs(pair.Key - ratio)).First();
        return (DynamicTemplateBase) Activator.CreateInstance(closestValue.Value);
    }
}
