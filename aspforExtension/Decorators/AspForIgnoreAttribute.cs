using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evergrowth.AspForMarkDigExtension.Decorators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AspForIgnoreAttribute : Attribute
{
    private bool _ignoreTotally = false;

    public AspForIgnoreAttribute()
    {
        this._ignoreTotally = true;
    }

    public bool IgnoreTotally
    {
        get
        {
            return _ignoreTotally;
        }
    }
}
