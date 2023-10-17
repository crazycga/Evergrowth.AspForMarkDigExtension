using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evergrowth.AspForMarkDigExtension.Decorators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AspForCheckedValueAttribute : Attribute
{
    private readonly string _value;

    public AspForCheckedValueAttribute(string value)
    {
        _value = value;
    }

    public string Value
    {
        get
        {
            return _value;
        }
    }
}
