using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evergrowth.AspForMarkDigExtension.Decorators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AspForRequiredInputAttribute : Attribute
{
    private bool _required = false;

    public AspForRequiredInputAttribute()
    {
        this._required = true;
    }

    public bool Required
    {
        get
        {
            return this._required;
        }
    }
}
