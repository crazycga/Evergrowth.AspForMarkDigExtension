using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evergrowth.AspForMarkDigExtension.Decorators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AspForReadOnlyAttribute : Attribute
{
    private bool _readOnly = false;

    public AspForReadOnlyAttribute()
    {
        this._readOnly = true;
    }

    public bool ReadOnly
    {
        get
        {
            return this._readOnly;
        }
    }
}
